#pragma once

#include <optional>

#include <PriorityMutex.hpp>
#include <ConditionVariable.hpp>
#include <Scheduler.hpp>

template<class Allocator>
class SafeAllocator
{
public:

	constexpr SafeAllocator(std::optional<opsy::IsrPriority> priority = std::nullopt, Allocator&& allocator = Allocator()) :
			m_allocator(std::move(allocator)), m_mutex(priority), m_memoryAvailable(priority)
	{

	}

	void* allocate(typename Allocator::size_type bytes)
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		return m_allocator.allocate(bytes);
	}

	template<typename T>
	T* allocate(typename Allocator::size_type count = 1)
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		return m_allocator.template allocate<T>(count);
	}

	void deallocate(const void *ptr)
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		auto previous = m_allocator.available();
		m_allocator.deallocate(ptr);
		auto now = m_allocator.available();
		if(now > previous)
			m_memoryAvailable.notify_all();
	}

	inline bool owns(const void *ptr) const
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		return m_allocator.owns(ptr);
	}

	inline constexpr typename Allocator::size_type size() const
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		return m_allocator.size();
	}

	inline constexpr typename Allocator::size_type available() const
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		return m_allocator.available();
	}

	inline constexpr bool empty() const
	{
		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		return m_allocator.empty();
	}

	opsy::ConditionVariable& memoryAvailable()
	{
		return m_memoryAvailable;
	}

	void* allocate(typename Allocator::size_type bytes, opsy::duration timeout)
	{
		const auto timeout_time = opsy::Scheduler::now() + timeout;

		std::lock_guard<opsy::PriorityMutex> lock (m_mutex);
		void* ptr = allocate(bytes); // try to allocate

		while(ptr == nullptr) // if not available
		{
			if(m_memoryAvailable.wait_until(m_mutex, timeout_time) == std::cv_status::timeout) // wait for more memory to be available
				return nullptr; // if timeout, bail out
			else
				ptr = allocate(bytes); // try to allocate again
		}

		return ptr;
	}

	template<typename T>
	T* allocate(typename Allocator::size_type count, opsy::duration timeout)
	{
		return reinterpret_cast<T*>(allocate(count*sizeof(T), timeout));
	}

public:

Allocator m_allocator;
opsy::PriorityMutex m_mutex;
opsy::ConditionVariable m_memoryAvailable;

};
