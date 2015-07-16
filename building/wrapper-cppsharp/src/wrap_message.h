#ifndef __wrapper__message__
#define __wrapper__message__

// system headers
#include <string>
#include <vector>
#include <map>

// Socket.IO headers
#include "sio_message.h"

// local headers
#include "wrap_message.h"

namespace SocketIO
{
    // forward declarations
    class Message;
    
    // enumerations
    enum MessageType
    {
        MessageTypeInteger,
        MessageTypeDouble,
        MessageTypeString,
        MessageTypeBinary,
        MessageTypeArray,
        MessageTypeObject
    };
    
    
    class Message
    {
    public:
        ~Message() { }
        
        MessageType GetFlag() const;
        long long GetInteger() const;
        double GetDouble() const;
        std::string const& GetString() const;
////        virtual shared_ptr<const string> const& get_binary() const
////        {
////            assert(false);
////            static shared_ptr<const string> s_empty_binary;
////            s_empty_binary = nullptr;
////            return s_empty_binary;
////        }
//        const std::vector<Message*>& GetArray() const;
//        std::vector<Message*>& GetArray();
//        const std::map<std::string, Message*>& GetObject() const;
//        std::map<std::string, Message*>& GetObject();
        
        static Message* Create(long long intValue)
        {
            return new Message(sio::int_message::create(intValue));
        }
        static Message* Create(double doubleValue)
        {
            return new Message(sio::double_message::create(doubleValue));
        }
        static Message* Create(std::string stringValue)
        {
            return new Message(sio::string_message::create(stringValue));
        }
        
    private:
        Message(sio::message::ptr msg) : m_impl(msg) { }
        static Message* Create(sio::message::ptr msg)
        {
            if (msg == nullptr)
                return NULL;
            else
                return new Message(msg);
        }
        
        sio::message::ptr m_impl;
        
        friend class Socket;
        friend class Event;
    };
}

#endif /* defined(__wrapper__message__) */
