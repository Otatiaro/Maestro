#pragma once

#include <cstdint>
#include <cstddef>
#include <type_traits>

#include "Buffer.hpp"

namespace maestro
{

template<class CommandType, typename SizeType>
class Packet
{

	static_assert(std::is_enum_v<CommandType>, "CommandType must be an enumeration");
	static_assert(std::is_same_v<std::underlying_type_t<CommandType>, uint8_t>, "The enumeration underlying type must be unsigned 8 bit integer");

public:

	using BufferType = Buffer<uint8_t, SizeType>;

	constexpr Packet(CommandType command) :
			m_command(command)
	{

	}

	constexpr Packet(CommandType command, typename BufferType::pointer data, SizeType size, typename BufferType::destructor_type destructor) :
			m_buffer(data, size, std::move(destructor)), m_command(command)
	{

	}

	constexpr Packet& operator=(Packet &&packet)
	{
		if (&packet != this) // check for self assignment
		{
			this->~Packet();
			m_command = packet.m_command;
			m_buffer = std::move(packet.m_buffer);
		}
		return *this;
	}

	constexpr Packet(Packet &&packet) :
			m_command(packet.m_command), m_buffer(std::move(packet.m_buffer))

	{

	}

	constexpr bool operator==(const Packet& compare) const
	{
		if(m_command != compare.m_command || m_buffer.size() != compare.m_buffer.size())
			return false;

		for (SizeType i = 0U; i < m_buffer.size(); ++i)
			if (m_buffer[i] != compare.m_buffer[i])
				return false;

		return true;
	}

	constexpr uint8_t checksum() const
	{
		uint8_t sum = m_command;

		for (auto i = 0U; i < sizeof(SizeType); ++i)
			sum = static_cast<uint8_t>(sum + (m_buffer.size() >> (8 * i)));

		for (const auto v : m_buffer)
			sum = static_cast<uint8_t>(sum + v);

		return static_cast<uint8_t>((~sum) + 1);
	}

	constexpr CommandType command() const
	{
		return m_command;
	}

	constexpr SizeType payloadSize() const
	{
		return m_buffer.size();
	}

	constexpr BufferType& payload()
	{
		return m_buffer;
	}

	constexpr const BufferType& payload() const
	{
		return m_buffer;
	}

private:

	BufferType m_buffer;
	CommandType m_command;
};
}
