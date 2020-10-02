#pragma once

#include "Callback.hpp"

namespace maestro
{
class IChannel
{
public:

	using ReceiveCallback = Callback<void(const uint8_t*, uint32_t)>;

	virtual void onReceived(ReceiveCallback&& callback) = 0;
	virtual void send(const uint8_t*, uint32_t) = 0;
};
}
