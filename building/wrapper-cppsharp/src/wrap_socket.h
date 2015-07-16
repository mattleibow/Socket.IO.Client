#ifndef __wrapper__socket__
#define __wrapper__socket__

// system headers
#include <string>
#include <vector>
#include <map>

// Socket.IO headers
#include "sio_socket.h"

// local headers
#include "wrap_socket.h"
#include "wrap_message.h"

namespace SocketIO
{
    // forward declarations
    class Event;
    class Socket;
    
    // typedefs
    typedef void (*EventListenerDelegate)(Event&);
    typedef void (*EventListenerAuxDelegate)(const std::string&, Message const&, bool, Message&);
    typedef void (*ErrorListenerDelegate)(Message const&);
    typedef void (*AckDelegate)(std::vector<Message> const&);
    
    
    class Event
    {
    public:
        ~Event();
        
        const std::string& GetNamespace() const;
        const std::string& GetName() const;
        const Message* GetMessage() const;
//        const std::vector<Message>& GetMessages() const;
        
        bool NeedAck() const;
        
        void SetAckMessage(Message* ack_message);
        Message* GetAckMessage() const;
        
    private:
        Event(sio::event* evt) : m_impl(evt) { }
        
        sio::event* m_impl;
        
        friend class Message;
        friend class Socket;
    };
    
    
    class Socket
    {
    public:
        ~Socket();
        
        void On(std::string const& eventName, EventListenerDelegate const& func);
        void On(std::string const& eventName, EventListenerAuxDelegate const& func);
        void Off(std::string const& eventName);
        void OffAll();
        
        void Close();
        
        void OnError(ErrorListenerDelegate const& l);
        void OffError();
        
        void Emit(std::string const& eventName);
        void Emit(std::string const& eventName, std::vector<Message> const& msgList);
        void Emit(std::string const& eventName, std::vector<Message> const& msgList, AckDelegate const& ack);
        
        std::string const& GetNamespace() const;
        
    private:
        Socket(sio::socket* sock) : m_impl(sock) { }
        
        //disable copy constructor and assign operator.
        Socket(Socket const& sock){}
        void operator=(Socket const& sock){}
        
        sio::socket* m_impl;
        
        friend class Client;
    };
}

#endif /* defined(__wrapper__socket__) */
