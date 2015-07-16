#ifndef __wrapper__client__
#define __wrapper__client__

// system headers
#include <string>
#include <vector>
#include <map>

// Socket.IO headers
#include "sio_client.h"

// local headers
#include "wrap_message.h"
#include "wrap_socket.h"

namespace SocketIO
{
    // forward declarations
    class Client;
    class Socket;
    
    // enumerations
    enum CloseReason
    {
        CloseReasonNormal,
        CloseReasonDrop
    };
    
    // typedefs
    typedef void (*ConnectionListenerDelegate)(void);
    typedef void (*CloseListenerDelegate)(CloseReason const&);
    typedef void (*ReconnectListenerDelegate)(unsigned int, unsigned int);
    typedef void (*SocketListenerDelegate)(std::string const&);
    
    class Client {
    public:
        Client();
        ~Client();
        
        // set listeners and event bindings
        void SetOpenListener(ConnectionListenerDelegate const& listener);
        void SetFailListener(ConnectionListenerDelegate const& listener);
        void SetReconnectingListener(ConnectionListenerDelegate const& listener);
        void SetReconnectListener(ReconnectListenerDelegate const& listener);
        void SetCloseListener(CloseListenerDelegate const& listener);
        void SetSocketOpenListener(SocketListenerDelegate const& listener);
        void SetSocketCloseListener(SocketListenerDelegate const& listener);
        // clear the listeners and event bindings
        void ClearConnectionListeners();
        void ClearSocketListeners();
        
        // client functions - such as send, etc
        void Connect(const std::string& uri);
        void Connect(const std::string& uri, const std::map<std::string, std::string>& query);
        void SetReconnectAttempts(int attempts);
        void SetReconnectDelay(unsigned int millis);
        void SetReconnectDelayMax(unsigned int millis);
        
        Socket const& GetSocket();
        Socket const& GetSocket(const std::string& nsp);
        
        // closes the connection
        void Close();
        void SyncClose();
        
        bool IsOpened() const;
        
        std::string const& GetSessionId() const;
        
    private:
        // disable copy constructor and assign operator
        Client(Client const& cl) { }
        void operator=(Client const& cl) { }
        
        sio::client* m_impl;
    };
}

//
//
//
//// extending types
//namespace sio {
//    %extend client {
//        void set_open_listener(con_listener_delegate const& l) {
//            $self->set_open_listener([l](void) {
//                l();
//            });
//        }
//        void set_fail_listener(con_listener_delegate const& l) {
//            $self->set_fail_listener([l](void) {
//                l();
//            });
//        }
//        void set_reconnecting_listener(con_listener_delegate const& l) {
//            $self->set_reconnecting_listener([l](void) {
//                l();
//            });
//        }
//        void set_reconnect_listener(reconnect_listener_delegate const& l) {
//            $self->set_reconnect_listener([l](unsigned reconn_made, unsigned delay) {
//                l(reconn_made, delay);
//            });
//        }
//        void set_close_listener(close_listener_delegate const& l) {
//            $self->set_close_listener([l](sio::client::close_reason const& reason) {
//                l(reason);
//            });
//        }
//        void set_socket_open_listener(socket_listener_delegate const& l) {
//            $self->set_socket_open_listener([l](std::string const& nsp) {
//                l(nsp);
//            });
//        }
//        void set_socket_close_listener(socket_listener_delegate const& l) {
//            $self->set_socket_close_listener([l](std::string const& nsp) {
//                l(nsp);
//            });
//        }
//    }
//    %extend socket {
//        void on(std::string const& event_name, event_listener_delegate const& func) {
//            $self->on(event_name, [func](sio::event& event) {
//                func(event);
//            });
//        }
//        void on(std::string const& event_name, event_listener_aux_delegate const& l) {
//            $self->on(event_name, [l](const std::string& name, sio::message::ptr const& message, bool need_ack, sio::message::ptr& ack_message) {
//                l(name, message, need_ack, ack_message);
//            });
//        }
//        void on_error(error_listener_delegate const& l) {
//            $self->on_error([l](sio::message::ptr const& message) {
//                l(message);
//            });
//        }
//        void emit(std::string const& name, sio::message::list const& msglist, ack_delegate const& ack) {
//            $self->emit(name, msglist, [ack](sio::message::list const& message_list) {
//                ack(message_list);
//            });
//        }
//    }
//}

#endif /* defined(__wrapper__client__) */
