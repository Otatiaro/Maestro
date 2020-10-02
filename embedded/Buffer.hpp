#pragma once

#include <cassert>
#include <cstddef>
#include <cstring>
#include <limits>

#include <Callback.hpp>

namespace maestro
{
/**
 * A buffer class
 * @tparam ItemType The type of the buffer items
 */
template<typename ItemType, typename SizeType = std::size_t>
class Buffer
{
public:

	// Types declaration for Container and ReverseContainer named requirements
	using destructor_type = opsy::Callback<void(ItemType*, SizeType)>;
	using value_type = ItemType;
	using reference = value_type&;
	using const_reference = const ItemType&;
	using iterator = ItemType*;
	using const_iterator = const ItemType*;
	using difference_type = std::ptrdiff_t;
	using size_type = SizeType;
	using reverse_iterator = std::reverse_iterator<iterator>;
	using const_reverse_iterator = std::reverse_iterator<const_iterator>;
	using pointer = ItemType*;
	using const_pointer = const ItemType*;


	/**
	 * Creates an empty buffer
	 */
	constexpr Buffer() :
			m_data(nullptr),
			m_size(0),
			m_destructor(destructor_type())
	{

	}

	/**
	 * Creates a buffer with the specified data, size and destructor
	 * @param data A pointer to the data
	 * @param size The size of the data (in terms of ItemType, not in bytes)
	 * @param destructor The destructor to call when the buffer is not in use anymore
	 */
	constexpr Buffer(pointer data, size_type size, destructor_type destructor) :
			m_data(data),
			m_size(size),
			m_destructor(std::move(destructor))
	{

	}

	/**
	 * The copy constructor is deleted, as it would require a new memory allocation, which is forbidden
	 * @param
	 */
	constexpr Buffer(const Buffer&) = delete;

	/**
	 * The copy assignment is deleted, as it would require a new memory allocation, which is forbidden
	 * @param
	 * @return
	 */
	constexpr Buffer& operator=(const Buffer&) = delete;

	/**
	 * Move constructor
	 * @param buffer The @c buffer to take the data from
	 */
	constexpr Buffer(Buffer&& buffer) :
			m_data(std::exchange(buffer.m_data, nullptr)),
			m_size(std::exchange(buffer.m_size, 0)),
			m_destructor(std::exchange(buffer.m_destructor, nullptr))
	{
	}

	/**
	 * Move assignment operator
	 * @param buffer The @c buffer to take data from
	 * @return A reference to the current @c buffer
	 */
	constexpr Buffer& operator=(Buffer&& buffer)
	{
		if(&buffer != this) // check for self assignment
		{
			this->~Buffer();
			m_data = std::exchange(buffer.m_data, nullptr);
			m_size = std::exchange(buffer.m_size, 0);
			m_destructor = std::move(buffer.m_destructor);
		}
		return *this;
	}

	~Buffer()
	{
		assert((m_data == nullptr) ^ (m_size != 0)); // either size is not zero or data is null pointer
		m_destructor(m_data, m_size);
		m_data = nullptr;
		m_size = 0;
	}

	/**
	 * Gets the reference of the item at specified rank
	 * @param pos The item rank
	 * @return The item at rank pos
	 */
	constexpr reference at(size_type pos)
	{
		assert(pos < size());
		return m_data[pos];
	}

	/**
	 * Gets the reference of the item at specified rank
	 * @param pos The item rank
	 * @return The item at rank pos
	 */
	constexpr const_reference at(size_type pos) const
	{
		assert(pos < size());
		return m_data[pos];
	}

	/**
	 * Gets the reference of the item at specified rank
	 * @param pos The item rank
	 * @return The item at rank pos
	 */
	constexpr reference operator[](size_type pos)
	{
		assert(pos < size());
		return m_data[pos];
	}

	/**
	 * Gets the reference of the item at specified rank
	 * @param pos The item rank
	 * @return The item at rank pos
	 */
	constexpr const_reference operator[](size_type pos) const
	{
		assert(pos < size());
		return m_data[pos];
	}

	/**
	 * Gets the reference of the item at the front of the container
	 * @return The item at the front of the container
	 */
	constexpr reference front()
	{
		assert(m_data != nullptr);
		return *m_data;
	}

	/**
	 * Gets the reference of the item at the front of the container
	 * @return The item at the front of the container
	 */
	constexpr const_reference front() const
	{
		assert(m_data != nullptr);
		return *m_data;
	}

	/**
	 * Gets the reference of the item at the back of the container
	 * @return The item at the back of the container
	 */
	constexpr reference back()
	{
		assert(m_size != 0 && m_data != nullptr);
		return m_data[m_size - 1];
	}

	/**
	 * Gets the reference of the item at the back of the container
	 * @return The item at the back of the container
	 */
	constexpr const_reference back() const
	{
		assert(m_size != 0 && m_data != nullptr);
		return m_data[m_size - 1];
	}

	/**
	 * Gets the pointer to the first item of the container
	 * @return The pointer to the first item in the container
	 */
	constexpr pointer data()
	{
		assert(m_data != nullptr);
		return m_data;
	}

	/**
	 * Gets the pointer to the first item of the container
	 * @return The pointer to the first item in the container
	 */
	constexpr const_pointer data() const
	{
		assert(m_data != nullptr);
		return m_data;
	}

	/**
	 * Checks if the container is empty (size is zero)
	 * @return @c true if the container is empty, @c false otherwise
	 */
	constexpr bool empty() const
	{
		return m_size == 0;
	}

	/**
	 * Gets the size of the data
	 * @return The size of the data (in terms of @c ItemType, not in bytes)
	 */
	constexpr size_type size() const
	{
		return m_size;
	}

	/**
	 * Gets the maximum size a container can be
	 * @return The maximum size a container can be
	 */
	constexpr size_type max_size() const
	{
		return std::numeric_limits<SizeType>::max();
	}

	/**
	 * Fills the data with the specified value
	 * @param value The value to copy in the data buffer
	 */
	constexpr void fill(const_reference value)
	{
		if constexpr (sizeof(ItemType) == 1) // in case the size of the item is one, use optimized library function
			std::memset(m_data, value, m_size);
		else
			for (auto i = 0U; i < m_size; ++i) // else copy one by one
				m_data[i] = value;
	}

	/**
	 * Gets an iterator at the beginning of the container
	 * @return An iterator at the beginning of the container
	 */
	constexpr const_iterator begin() const
	{
		return iterator(m_data);
	}

	/**
	 * Gets an iterator at the beginning of the container
	 * @return An iterator at the beginning of the container
	 */
	constexpr iterator begin()
	{
		return iterator(m_data);
	}

	/**
	 * Gets a constant iterator at the beginning of the container
	 * @return A constant iterator at the beginning of the container
	 */
	constexpr const_iterator cbegin() const
	{
		return const_iterator(m_data);
	}

	/**
	 * Gets an iterator past the end of the container
	 * @return An iterator past the end of the container
	 */
	constexpr const_iterator end() const
	{
		return iterator(&m_data[m_size]);
	}

	/**
	 * Gets an iterator past the end of the container
	 * @return An iterator past the end of the container
	 */
	constexpr iterator end()
	{
		return iterator(&m_data[m_size]);
	}

	/**
	 * Gets a constant iterator past the end of the container
	 * @return A constant iterator past the end of the container
	 */
	constexpr const_iterator cend() const
	{
		return const_iterator(&m_data[m_size]);
	}

	/**
	 * Gets an iterator at the beginning of the container in reverse order
	 * @return An iterator at the beginning of the container in reverse order
	 */
	constexpr reverse_iterator rbegin()
	{
		return iterator(&m_data[m_size - 1]);
	}

	/**
	 * Gets an iterator past the end of the container in reverse order
	 * @return An iterator past the end of the container in reverse order
	 */
	constexpr reverse_iterator rend()
	{
		return iterator(&m_data[-1]);
	}

	/**
	 * Gets an iterator at the beginning of the container in reverse order
	 * @return An iterator at the beginning of the container in reverse order
	 */
	constexpr const_reverse_iterator rbegin() const
	{
		return iterator(&m_data[m_size - 1]);
	}

	/**
	 * Gets an iterator past the end of the container in reverse order
	 * @return An iterator past the end of the container in reverse order
	 */
	constexpr const_reverse_iterator rend() const
	{
		return iterator(&m_data[-1]);
	}

	/**
	 * Gets a constant iterator at the beginning of the container in reverse order
	 * @return A constant iterator at the beginning of the container in reverse order
	 */
	constexpr const_reverse_iterator crbegin() const
	{
		return const_iterator(&m_data[m_size -1]);
	}

	/**
	 * Gets a constant iterator past the end of the container in reverse order
	 * @return A constant iterator past the end of the container in reverse order
	 */
	constexpr const_reverse_iterator crend() const
	{
		assert(m_data != nullptr);
		return const_iterator(&m_data[-1]);
	}

protected:

	pointer m_data;
	size_type m_size;
	destructor_type m_destructor;
};
}
