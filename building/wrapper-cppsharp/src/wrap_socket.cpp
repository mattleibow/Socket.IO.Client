// system headers
#include <string>
#include <vector>
#include <map>

// Socket.IO headers
#include "sio_client.h"
#include "sio_socket.h"
#include "sio_message.h"

// local headers
#include "wrap_client.h"
#include "wrap_message.h"
#include "wrap_socket.h"

namespace SocketIO
{
    Event::~Event()
    {
        delete m_impl;
    }
    
    const std::string& Event::GetNamespace() const
    {
        return m_impl->get_nsp();
    }
    
    const std::string& Event::GetName() const
    {
        return m_impl->get_name();
    }
    
    const Message* Event::GetMessage() const
    {
        return Message::Create(m_impl->get_message());
    }
    
//    const std::vector<Message>& Event::GetMessages() const
//    {
//        return m_impl->get_messages();
//    }
    
    bool Event::NeedAck() const
    {
        return m_impl->need_ack();
    }
    
    void Event::SetAckMessage(Message* ack_message)
    {
        m_impl->put_ack_message(ack_message->m_impl);
    }
    
    Message* Event::GetAckMessage() const
    {
        return Message::Create(m_impl->get_ack_message());
    }
    
    
    Socket::~Socket()
    {
        delete m_impl;
    }
    
    void Socket::On(std::string const& eventName, EventListenerDelegate const& func)
    {
        m_impl->on(eventName, [func](sio::event& event) {
            func(*new Event(&event));
        });
    }
    
    void Socket::On(std::string const& eventName, EventListenerAuxDelegate const& func)
    {
        m_impl->on(eventName, [func](const std::string& name, sio::message::ptr const& message, bool need_ack, sio::message::ptr& ack_message) {
            func(name, *Message::Create(message), need_ack, *Message::Create(ack_message));
        });
    }
    
    void Socket::Off(std::string const& eventName)
    {
        m_impl->off(eventName);
    }
    
    void Socket::OffAll()
    {
        m_impl->off_all();
    }
    
    void Socket::Close()
    {
        m_impl->close();
    }
    
    void Socket::OnError(ErrorListenerDelegate const& listener)
    {
        m_impl->on_error([listener](sio::message::ptr const& message) {
            listener(*Message::Create(message));
        });
    }
    
    void Socket::OffError()
    {
        m_impl->off_error();
    }
    
    void Socket::Emit(std::string const& eventName)
    {
        m_impl->emit(eventName, nullptr, NULL);
    }
    
    void Socket::Emit(std::string const& eventName, std::vector<Message> const& msgList)
    {
//        m_impl->emit(eventName, msgList, NULL);
    }
    
    void Socket::Emit(std::string const& eventName, std::vector<Message> const& msgList, AckDelegate const& ack)
    {
//        m_impl->emit(eventName, msgList, ack);
    }
    
    std::string const& Socket::GetNamespace() const
    {
        return m_impl->get_namespace();
    }
}
