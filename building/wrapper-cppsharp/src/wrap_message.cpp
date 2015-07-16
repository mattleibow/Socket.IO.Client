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
    MessageType Message::GetFlag() const
    {
        return (MessageType)m_impl->get_flag();
    }
    
    long long Message::GetInteger() const
    {
        if (m_impl->get_flag() == sio::message::flag_integer)
        {
            return m_impl->get_int();
        }
        
        return 0;
    }
    
    double Message::GetDouble() const
    {
        if (m_impl->get_flag() == sio::message::flag_integer ||
            m_impl->get_flag() == sio::message::flag_double)
        {
            return m_impl->get_double();
        }
        
        return 0;
    }
    
    std::string const& Message::GetString() const
    {
        if (m_impl->get_flag() == sio::message::flag_string)
        {
            return m_impl->get_string();
        }
        
        static std::string empty;
        empty.clear();
        return empty;
    }
    
//    const std::vector<Message*>& Message::GetArray() const
//    {
//        if (m_impl->get_flag() == sio::message::flag_array)
//        {
//            auto array = m_impl->get_vector();
//            std::vector<Message*>* newArray = new std::vector<Message*>();
//            for (int i = 0; i < array.size(); i++) {
//                newArray->push_back(Message::Create(array[i]));
//            }
//            return *newArray;
//        }
//        
//        static std::vector<Message*> empty;
//        empty.clear();
//        return empty;
//    }
//    
//    std::vector<Message*>& Message::GetArray()
//    {
//        if (m_impl->get_flag() == sio::message::flag_array)
//        {
//            return m_impl->get_vector();
//        }
//        
//        static std::vector<Message*> empty;
//        empty.clear();
//        return empty;
//    }
//    
//    const std::map<std::string, Message*>& Message::GetObject() const
//    {
//        if (m_impl->get_flag() == sio::message::flag_object)
//        {
//            return m_impl->get_map();
//        }
//        
//        static std::map<std::string, Message*> empty;
//        empty.clear();
//        return empty;
//    }
//    
//    std::map<std::string, Message*>& Message::GetObject()
//    {
//        if (m_impl->get_flag() == sio::message::flag_object)
//        {
//            return m_impl->get_map();
//        }
//        
//        static std::map<std::string, Message*> empty;
//        empty.clear();
//        return empty;
//    }
}
