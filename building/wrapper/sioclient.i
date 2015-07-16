%module sioclient

%{
    // system headers
    #include <string>
    #include <memory>
    #include <vector>
    #include <map>
    #include <cassert>
    #include <type_traits>
    #include <functional>
    
    // local headers
    #include "../../out/include/sio_client.h"
    #include "../../out/include/sio_message.h"
    #include "../../out/include/sio_socket.h"
    
//    // the C functions
//    typedef void (*con_listener_delegate)(void);
//    typedef void (*close_listener_delegate)(sio::client::close_reason const&);
//    typedef void (*reconnect_listener_delegate)(unsigned, unsigned);
//    typedef void (*socket_listener_delegate)(std::string const&);
//    typedef void (*event_listener_aux_delegate)(const std::string&, sio::message::ptr const&, bool, sio::message::ptr&);
//    typedef void (*event_listener_delegate)(sio::event&);
//    typedef void (*error_listener_delegate)(sio::message::ptr const&);
//    typedef void (*ack_delegate)(sio::message::list const&);
//    // the same for the iOS proxies
    typedef void (*ConnectionListenerDelegateInternal)(void*);
    typedef void (*CloseListenerDelegateInternal)(sio::client::close_reason const&, void*);
    typedef void (*ReconnectListenerDelegateInternal)(unsigned, unsigned, void*);
    typedef void (*SocketListenerDelegateInternal)(std::string const&, void*);
    typedef void (*EventListenerAuxDelegateInternal)(const std::string&, sio::message::ptr const&, bool, sio::message::ptr&, void*);
    typedef void (*EventListenerDelegateInternal)(sio::event&, void*);
    typedef void (*ErrorListenerDelegateInternal)(sio::message::ptr const&, void*);
    typedef void (*AckDelegateInternal)(sio::message::list const&, void*);
//    // end iOS proxies
%}

%include "stl.i"
%include "std_shared_ptr.i"
%include "attribute.i"

// we want partial classes
%typemap(csclassmodifiers) sio::event "public partial class";
%typemap(csclassmodifiers) sio::client "public partial class";
%typemap(csclassmodifiers) sio::socket "public partial class";
%typemap(csclassmodifiers) sio::message "public partial class";
%typemap(csclassmodifiers) sio::message::list "public partial class";
%typemap(csclassmodifiers) std::vector<std::shared_ptr<sio::message>> "public partial class";
%typemap(csclassmodifiers) std::map<std::string, std::shared_ptr<sio::message>> "public partial class";
%typemap(csclassmodifiers) std::map<std::string, std::string> "public partial class";
%typemap(csclassmodifiers) std::map<std::string, std::string> "public partial class";
// derived types are internal due to SWIG limitation
%typemap(csclassmodifiers) sio::array_message "internal partial class";
%typemap(csclassmodifiers) sio::double_message "internal partial class";
%typemap(csclassmodifiers) sio::int_message "internal partial class";
%typemap(csclassmodifiers) sio::object_message "internal partial class";
%typemap(csclassmodifiers) sio::string_message "internal partial class";

// make sure we make the iOS proxy methods internal
%csmethodmodifiers sio::client::SetConnectionOpenListener "internal";
%csmethodmodifiers sio::client::SetConnectionFailListener "internal";
%csmethodmodifiers sio::client::SetReconnectingListener "internal";
%csmethodmodifiers sio::client::SetReconnectListener "internal";
%csmethodmodifiers sio::client::SetConnectionCloseListener "internal";
%csmethodmodifiers sio::client::SetSocketOpenListener "internal";
%csmethodmodifiers sio::client::SetSocketCloseListener "internal";
%csmethodmodifiers sio::socket::On "internal";
%csmethodmodifiers sio::socket::OnError "internal";
%csmethodmodifiers sio::socket::Emit "internal";
%csmethodmodifiers sio::message::list::at "internal";

// handling void* to IntPtr
%typemap(ctype)  void * "void *"
%typemap(imtype) void * "global::System.IntPtr"
%typemap(cstype) void * "global::System.IntPtr"
%typemap(csin)   void * "$csinput"
%typemap(in)     void * %{ $1 = $input; %}
%typemap(out)    void * %{ $result = $1; %}
%typemap(csout)  void * { return $imcall; }

// delegate types
%define %cs_callback(TYPE, CSTYPE)
    %typemap(ctype) TYPE, TYPE& "void*"
    %typemap(in) TYPE  %{ $1 = (TYPE)$input; %}
    %typemap(in) TYPE& %{ $1 = (TYPE*)&$input; %}
    %typemap(imtype, out="IntPtr") TYPE, TYPE& "CSTYPE"
    %typemap(cstype, out="IntPtr") TYPE, TYPE& "CSTYPE"
    %typemap(csin) TYPE, TYPE& "$csinput"
%enddef
%define %cs_callback2(TYPE, CTYPE, CSTYPE)
    %typemap(ctype) TYPE "CTYPE"
    %typemap(in) TYPE %{ $1 = (TYPE)$input; %}
    %typemap(imtype, out="IntPtr") TYPE "CSTYPE"
    %typemap(cstype, out="IntPtr") TYPE "CSTYPE"
    %typemap(csin) TYPE "$csinput"
%enddef

// shared pointers
//%shared_ptr(std::string)
//%shared_ptr(std::string const)
%shared_ptr(sio::message)
%shared_ptr(sio::int_message)
%shared_ptr(sio::double_message)
%shared_ptr(sio::string_message)
%shared_ptr(sio::binary_message)
%shared_ptr(sio::array_message)
%shared_ptr(sio::object_message)
%shared_ptr(sio::socket)

// template mapping
%template(MessageList) std::vector<std::shared_ptr<sio::message>>;
%template(MessageDictionary) std::map<std::string, std::shared_ptr<sio::message>>;
%template(StringDictionary) std::map<std::string, std::string>;

// ignore
%ignore sio::message::list::list(string &&);
%ignore sio::string_message::create(string &&);
%ignore sio::message::list::list(nullptr_t);
// wrap the std::function with C function pointers
%ignore sio::client::set_open_listener(con_listener const&);
%ignore sio::client::set_fail_listener(con_listener const&);
%ignore sio::client::set_reconnecting_listener(con_listener const&);
%ignore sio::client::set_reconnect_listener(reconnect_listener const&);
%ignore sio::client::set_close_listener(close_listener const&);
%ignore sio::client::set_socket_open_listener(socket_listener const&);
%ignore sio::client::set_socket_close_listener(socket_listener const&);
%ignore sio::socket::on(std::string const&, event_listener const&);
%ignore sio::socket::on(std::string const&, event_listener_aux const&);
%ignore sio::socket::on_error(error_listener const&);
%ignore sio::socket::emit(std::string const&, message::list const&, std::function<void (message::list const&)> const&);
// remove binary temporarily
%ignore sio::binary_message;
%ignore sio::message::get_binary() const;
%ignore sio::message::list::list(shared_ptr<string> const&);
%ignore sio::message::list::list(shared_ptr<const string> const&);

//// map the delegates
//%cs_callback(con_listener_delegate, ConnectionListenerDelegate);
//%cs_callback(close_listener_delegate, CloseListenerDelegate);
//%cs_callback(reconnect_listener_delegate, ReconnectListenerDelegate);
//%cs_callback(socket_listener_delegate, SocketListenerDelegate);
//%cs_callback(event_listener_delegate, EventListenerDelegate);
//%cs_callback(event_listener_aux_delegate, EventListenerAuxDelegate);
//%cs_callback(error_listener_delegate, ErrorListenerDelegate);
//%cs_callback(ack_delegate, AckDelegate);
//// for iOS proxies
%cs_callback(ConnectionListenerDelegateInternal, ConnectionListenerDelegateInternal);
%cs_callback(CloseListenerDelegateInternal, CloseListenerDelegateInternal);
%cs_callback(ReconnectListenerDelegateInternal, ReconnectListenerDelegateInternal);
%cs_callback(SocketListenerDelegateInternal, SocketListenerDelegateInternal);
%cs_callback(EventListenerDelegateInternal, EventListenerDelegateInternal);
%cs_callback(EventListenerAuxDelegateInternal, EventListenerAuxDelegateInternal);
%cs_callback(ErrorListenerDelegateInternal, ErrorListenerDelegateInternal);
%cs_callback(AckDelegateInternal, AckDelegateInternal);
// end iOS proxies

// renaming members
%rename(Create) sio::message::create;
%rename(GetInt) sio::message::get_int;
%rename(GetDouble) sio::message::get_double;
%rename(GetString) sio::message::get_string;
%rename(GetObject) sio::message::get_map;
%rename(GetArray) sio::message::get_vector;
%rename(GetItem) sio::message::list::at;
%rename(Insert) sio::message::list::insert;
%rename(Add) sio::message::list::push;
%rename(ToArrayMessage) sio::message::list::to_array_message;
%rename(On) sio::socket::on;
%rename(Off) sio::socket::off;
%rename(OffAll) sio::socket::off_all;
%rename(Close) sio::socket::close;
%rename(OnError) sio::socket::on_error;
%rename(OffError) sio::socket::off_error;
%rename(Emit) sio::socket::emit;
%rename(ClearConnectionListeners) sio::client::clear_con_listeners;
%rename(ClearSocketListeners) sio::client::clear_socket_listeners;
%rename(Connect) sio::client::connect;
%rename(Close) sio::client::close;
%rename(CloseSync) sio::client::sync_close;
%rename(SetReconnectAttempts) sio::client::set_reconnect_attempts;
%rename(SetReconnectDelay) sio::client::set_reconnect_delay;
%rename(SetReconnectDelayMax) sio::client::set_reconnect_delay_max;
%rename(GetSocket) sio::client::socket;

// renaming types
%rename(SocketEvent) sio::event;
%rename(Client) sio::client;
%rename(Socket) sio::socket;
%rename(List) sio::message::list;
%rename(MessageType) sio::message::flag;
%rename(Message) sio::message;
%rename(ArrayMessage) sio::array_message;
%rename(DoubleMessage) sio::double_message;
%rename(IntMessage) sio::int_message;
%rename(ObjectMessage) sio::object_message;
%rename(StringMessage) sio::string_message;
%rename(CloseReason) sio::client::close_reason;

// map the properties
%attribute(sio::event, sio::message::ptr const&, AckMessage, get_ack_message, put_ack_message);
%attribute(sio::event, std::string, Namespace, get_nsp);
%attribute(sio::event, std::string, Name, get_name);
%attribute(sio::event, sio::message::ptr&, Message, get_message);
%attribute(sio::event, sio::message::list&, Messages, get_messages);
%attribute(sio::event, bool, NeedAck, need_ack);
%attribute(sio::socket, std::string, Namespace, get_namespace);
%attribute(sio::client, std::string const&, SessionId, get_sessionid);
%attribute(sio::client, bool, IsOpen, opened);
%attribute(sio::message, sio::message::flag, Type, get_flag);
%attribute(sio::message::list, size_t, Count, size);

// API definitions
// other types
typedef	long long int64_t;
//// the C functions
//typedef void (*con_listener_delegate)(void);
//typedef void (*close_listener_delegate)(sio::client::close_reason const&);
//typedef void (*reconnect_listener_delegate)(unsigned, unsigned);
//typedef void (*socket_listener_delegate)(std::string const&);
//typedef void (*event_listener_aux_delegate)(const std::string&, sio::message::ptr const&, bool, sio::message::ptr&);
//typedef void (*event_listener_delegate)(sio::event&);
//typedef void (*error_listener_delegate)(sio::message::ptr const&);
//typedef void (*ack_delegate)(sio::message::list const&);
//// the same for the iOS proxies
typedef void (*ConnectionListenerDelegateInternal)(void*);
typedef void (*CloseListenerDelegateInternal)(sio::client::close_reason const&, void*);
typedef void (*ReconnectListenerDelegateInternal)(unsigned, unsigned, void*);
typedef void (*SocketListenerDelegateInternal)(std::string const&, void*);
typedef void (*EventListenerAuxDelegateInternal)(const std::string&, sio::message::ptr const&, bool, sio::message::ptr&, void*);
typedef void (*EventListenerDelegateInternal)(sio::event&, void*);
typedef void (*ErrorListenerDelegateInternal)(sio::message::ptr const&, void*);
typedef void (*AckDelegateInternal)(sio::message::list const&, void*);
//// end iOS proxies
// include files
%include "../out/include/sio_client.h"
%include "../out/include/sio_message.h"
%include "../out/include/sio_socket.h"


// extending types for delegates
namespace sio {
    %extend client {
        //// map C++ function<> to C# delegates
        //void SetConnectionOpenListener(con_listener_delegate const& func) {
        //    $self->set_open_listener(func);
        //}
        //void SetConnectionFailListener(con_listener_delegate const& func) {
        //    $self->set_fail_listener(func);
        //}
        //void SetReconnectingListener(con_listener_delegate const& func) {
        //    $self->set_reconnecting_listener(func);
        //}
        //void SetReconnectListener(reconnect_listener_delegate const& func) {
        //    $self->set_reconnect_listener(func);
        //}
        //void SetConnectionCloseListener(close_listener_delegate const& func) {
        //    $self->set_close_listener(func);
        //}
        //void SetSocketOpenListener(socket_listener_delegate const& func) {
        //    $self->set_socket_open_listener(func);
        //}
        //void SetSocketCloseListener(socket_listener_delegate const& func) {
        //    $self->set_socket_close_listener(func);
        //}
        //// for iOS proxies
        void SetConnectionOpenListener(ConnectionListenerDelegateInternal const& func, void* userdata) {
            $self->set_open_listener([func, userdata](void){
                func(userdata);
            });
        }
        void SetConnectionFailListener(ConnectionListenerDelegateInternal const& func, void* userdata) {
            $self->set_fail_listener([func, userdata](void){
                func(userdata);
            });
        }
        void SetReconnectingListener(ConnectionListenerDelegateInternal const& func, void* userdata) {
            $self->set_reconnecting_listener([func, userdata](void){
                func(userdata);
            });
        }
        void SetReconnectListener(ReconnectListenerDelegateInternal const& func, void* userdata) {
            $self->set_reconnect_listener([func, userdata](unsigned int reconn_made, unsigned int delay){
                func(reconn_made, delay, userdata);
            });
        }
        void SetConnectionCloseListener(CloseListenerDelegateInternal const& func, void* userdata) {
            $self->set_close_listener([func, userdata](sio::client::close_reason const& reason){
                func(reason, userdata);
            });
        }
        void SetSocketOpenListener(SocketListenerDelegateInternal const& func, void* userdata) {
            $self->set_socket_open_listener([func, userdata](std::string const& nsp){
                func(nsp, userdata);
            });
        }
        void SetSocketCloseListener(SocketListenerDelegateInternal const& func, void* userdata) {
            $self->set_socket_close_listener([func, userdata](std::string const& nsp){
                func(nsp, userdata);
            });
        }
        //// end iOS proxies
    }
    %extend socket {
        //// map C++ function<> to C# delegates
        //void On(std::string const& event_name, event_listener_delegate const& func) {
        //    $self->on(event_name, func);
        //}
        //void On(std::string const& event_name, event_listener_aux_delegate const& func) {
        //    $self->on(event_name, func);
        //}
        //void OnError(error_listener_delegate const& func) {
        //    $self->on_error(func);
        //}
        //void Emit(std::string const& name, sio::message::list const& msglist, ack_delegate const& ack) {
        //    $self->emit(name, msglist, ack);
        //}
        //// for iOS proxies
        void On(std::string const& event_name, EventListenerDelegateInternal const& func, void* userdata) {
            $self->on(event_name, [func, userdata](sio::event& event){
                func(event, userdata);
            });
        }
        void On(std::string const& event_name, EventListenerAuxDelegateInternal const& func, void* userdata) {
            $self->on(event_name, [func, userdata](const std::string& name, sio::message::ptr const& message, bool need_ack, sio::message::ptr& ack_message){
                func(name, message, need_ack, ack_message, userdata);
            });
        }
        void OnError(ErrorListenerDelegateInternal const& func, void* userdata) {
            $self->on_error([func, userdata](sio::message::ptr const& message){
                func(message, userdata);
            });
        }
        void Emit(std::string const& name, sio::message::list const& msglist, AckDelegateInternal const& ack, void* userdata) {
            $self->emit(name, msglist, [ack, userdata](sio::message::list const& message_list){
                ack(message_list, userdata);
            });
        }
        //// end iOS proxies
    }
}
