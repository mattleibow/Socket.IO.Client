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
    Client::Client()
        : m_impl(new sio::client())
    {
    }
    
    Client::~Client()
    {
        delete m_impl;
    }
    
    void Client::SetOpenListener(ConnectionListenerDelegate const& listener)
    {
        m_impl->set_open_listener(listener);
    }
    
    void Client::SetFailListener(ConnectionListenerDelegate const& listener)
    {
        m_impl->set_fail_listener(listener);
    }
    
    void Client::SetCloseListener(CloseListenerDelegate const& listener)
    {
        m_impl->set_close_listener([listener](sio::client::close_reason reason) {
            listener((CloseReason)reason);
        });
    }
    
    void Client::SetSocketOpenListener(SocketListenerDelegate const& listener)
    {
        m_impl->set_socket_open_listener(listener);
    }
    
    void Client::SetReconnectListener(ReconnectListenerDelegate const& listener)
    {
        m_impl->set_reconnect_listener(listener);
    }
    
    void Client::SetReconnectingListener(ConnectionListenerDelegate const& listener)
    {
        m_impl->set_reconnecting_listener(listener);
    }
    
    void Client::SetSocketCloseListener(SocketListenerDelegate const& listener)
    {
        m_impl->set_socket_close_listener(listener);
    }
    
    void Client::ClearConnectionListeners()
    {
        m_impl->clear_con_listeners();
    }
    
    void Client::ClearSocketListeners()
    {
        m_impl->clear_socket_listeners();
    }
    
    void Client::Connect(const std::string& uri)
    {
        const std::map<std::string, std::string> query;
        m_impl->connect(uri, query);
    }
    
    void Client::Connect(const std::string& uri, const std::map<std::string, std::string>& query)
    {
        m_impl->connect(uri, query);
    }
    
    Socket const& Client::GetSocket()
    {
        return *new Socket(m_impl->socket("").get());
    }
    
    Socket const& Client::GetSocket(const std::string& nsp)
    {
        return *new Socket(m_impl->socket(nsp).get());
    }
    
    void Client::Close()
    {
        m_impl->close();
    }
    
    void Client::SyncClose()
    {
        m_impl->sync_close();
    }
    
    bool Client::IsOpened() const
    {
        return m_impl->opened();
    }
    
    std::string const& Client::GetSessionId() const
    {
        return m_impl->get_sessionid();
    }
    
    void Client::SetReconnectAttempts(int attempts)
    {
        m_impl->set_reconnect_attempts(attempts);
    }
    
    void Client::SetReconnectDelay(unsigned millis)
    {
        m_impl->set_reconnect_delay(millis);
    }
    
    void Client::SetReconnectDelayMax(unsigned millis)
    {
        m_impl->set_reconnect_delay_max(millis);
    }
}
