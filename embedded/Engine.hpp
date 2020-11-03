#pragma once

#include <chrono>

#include <Callback.hpp>
#include "Packet.hpp"
#include "PacketStream.hpp"

using namespace std::chrono_literals;

namespace maestro
{

enum class Command : uint8_t
{
	ProtocolVersion = 0,
	Descriptor = 1,
	PackageData = 2,
	GetValues = 3,
	SetValues = 4,
	Reset = 5,
	CopySetup = 6,
	ActiveView = 7,
	ShowMessage = 8,
	MessageResult = 9,
	PluginCommand = 10,
};

template<typename Command, typename SizeType>
class IChannel
{
public:
	using PacketType = Packet<Command, SizeType>;

	virtual void send(PacketType&&) = 0;
};

template<class Allocator, class Configuration>
class Engine
{
	static constexpr uint8_t ProtocolVersion = 4;
	static_assert(Configuration::ProtocolVersion == ProtocolVersion, "This version supports only version 4");

public:

	using SizeType = uint16_t;
	using PacketType = Packet<Command, SizeType>;
	using Channel = IChannel<Command, SizeType>;

	constexpr Engine(Allocator &allocator, Configuration& configuration) :
			m_allocator(allocator), m_configuration(configuration)
	{

	}

	void process(PacketType&& packet, Channel& channel)
	{
		switch(packet.command())
		{
		case Command::ProtocolVersion:
		{
			auto reply = makePacket(Command::ProtocolVersion, 1);
			if(reply.payloadSize() == 1)
			{
				reply.payload()[0] = ProtocolVersion;
				channel.send(std::move(reply));
			}
			break;
		}
		case Command::Descriptor:
		{
			auto reply = makePacket(Command::Descriptor, Configuration::Descriptor.size());
			if(reply.payloadSize() == Configuration::Descriptor.size())
			{
				memcpy(reply.payload().data(), Configuration::Descriptor.data(), Configuration::Descriptor.size());
				channel.send(std::move(reply));
			}
			break;
		}
		case Command::PackageData:
		{

		}
		}
	}

private:

	Allocator &m_allocator;
	Configuration& m_configuration;

	PacketType makePacket(Command command, SizeType size)
	{
		auto ptr = m_allocator.template allocate<uint8_t>(size, 1s);

		if(ptr == nullptr)
			return PacketType{command,nullptr, 0, [](uint8_t*, SizeType){}};
		else
			return PacketType{command, ptr, size, [&](uint8_t* ptr, SizeType){m_allocator.deallocate(ptr);}};
	}
};
}
