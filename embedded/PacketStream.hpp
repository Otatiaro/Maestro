#pragma once

#include <cassert>

#include <Callback.hpp>
#include "Packet.hpp"

namespace maestro
{

template<typename CommandType, class Allocator, typename SizeType, uint8_t StartByte = 0x55, uint8_t EndByte = 0x3C, uint8_t EscapeByte = 0xC3>
class PacketStream
{
public:

	using PacketType = Packet<CommandType, SizeType>;

	using PacketReceivedCallback = opsy::Callback<void (PacketType&&)>;

	PacketStream(Allocator &allocator, PacketReceivedCallback callback) :
			m_allocator(allocator), m_callback(std::move(callback))
	{

	}

	static constexpr std::size_t rawSize(const PacketType& packet)
	{
		std::size_t size = 5 + // 2 start bytes + 1 command byte + 1 checksum byte + 1 end byte
			sizeof(SizeType) + // size field
			packet.payloadSize() + // not escaped count
			isSpecial(static_cast<uint8_t>(packet.command())); // +1 if command is special byte

		auto tmp = packet.payloadSize();

		for(auto i = 0U; i<sizeof(SizeType); ++i)
		{
			size += isSpecial(static_cast<uint8_t>(tmp));
			tmp >>= 8;
		}

		for(auto v : packet.payload())
			size += isSpecial(v); // if value is special value, it needs to be escaped first

		return size;
	}

	static constexpr bool isSpecial(uint8_t value)
	{
		return (value == StartByte) || (value == EndByte) || (value == EscapeByte);
	}

	template<class RangeType>
	constexpr static void writeTo(const PacketType& packet, RangeType&& range)
	{
		*range++ = StartByte;
		*range++ = StartByte;
		writeEscapedByte(range, static_cast<uint8_t>(packet.command()));

		for (auto i = 0U; i < sizeof(SizeType); ++i)
			writeEscapedByte(range, static_cast<uint8_t>(packet.payloadSize() >> (8 * (sizeof(SizeType) - 1 - i))));

		for(auto v : packet.payload())
			writeEscapedByte(range, v);

		writeEscapedByte(range, packet.checksum());
		*range++ = EndByte;
	}

	void feed(uint8_t value)
	{
		if (value == EscapeByte && !m_escaping && (m_state != Start && m_state != SecondStart && m_state != End))
		{
			m_escaping = true;
			return;
		}
		else
		{
			m_escaping = false;
		}

		switch (m_state)
		{
		case Start:
		{
			if (value == StartByte)
				m_state = SecondStart;
			break;
		}
		case SecondStart:
		{
			m_state = (value == StartByte) ? Command : Start;
			break;
		}
		case Command:
		{
			m_lastCommand = static_cast<CommandType>(value);
			m_index = 0;
			m_size = 0;
			m_state = Size;
			break;
		}
		case Size:
		{
			m_size = static_cast<SizeType>((m_size << 8) | value);
			if (++m_index == sizeof(SizeType))
			{
				if (m_size == 0)
				{
					m_packet = PacketType{ m_lastCommand, nullptr, 0, [](uint8_t*, SizeType){} };
					m_state = Checksum;
				}
				else
				{
					uint8_t *ptr = reinterpret_cast<uint8_t*>(m_allocator.allocate(m_size));
					if (ptr == nullptr)
						m_state = Start;
					else
					{
						m_packet = PacketType{ m_lastCommand, ptr, m_size, [&](uint8_t *p, SizeType){m_allocator.deallocate(p);} };
						m_index = 0;
						m_state = Payload;
					}
				}
			}
			break;
		}
		case Payload:
		{
			m_packet.payload()[m_index++] = value;
			if(m_index == m_packet.payloadSize())
				m_state = Checksum;
			break;
		}
		case Checksum:
		{
			m_state = value == m_packet.checksum() ? End : Start;
			break;
		}
		case End:
		{
			if(value == EndByte)
				m_callback(std::move(m_packet));
			m_state = Start;
			break;
		}
		}
	}

private:

	enum WaitState
	{
		Start, SecondStart, Command, Size, Payload, Checksum, End,
	};

	Allocator &m_allocator;
	PacketType m_packet { static_cast<CommandType>(0) };
	PacketReceivedCallback m_callback;
	WaitState m_state { Start };
	bool m_escaping = false;
	CommandType m_lastCommand;
	SizeType m_size;
	uint16_t m_index;

	template<class RangeType>
	static constexpr void writeEscapedByte(RangeType&& range, uint8_t value)
	{
		if(isSpecial(value))
			*range++ = EscapeByte;
		*range++ = value;
	}
};
}
