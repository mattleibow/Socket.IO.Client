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
    
    typedef void (*con_listener_delegate)(void);
    typedef void (*close_listener_delegate)(sio::client::close_reason const&);
    typedef void (*reconnect_listener_delegate)(unsigned, unsigned);
    typedef void (*socket_listener_delegate)(std::string const&);
    typedef void (*event_listener_aux_delegate)(const std::string&, sio::message::ptr const&, bool, sio::message::ptr&);
    typedef void (*event_listener_delegate)(sio::event&);
    typedef void (*error_listener_delegate)(sio::message::ptr const&);
    typedef void (*ack_delegate)(sio::message::list const&);
    
%}

%include "stl.i"
%include "std_shared_ptr.i"

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
%shared_ptr(std::string)
%shared_ptr(std::string const)
%shared_ptr(sio::message)
%shared_ptr(sio::int_message)
%shared_ptr(sio::double_message)
%shared_ptr(sio::string_message)
%shared_ptr(sio::binary_message)
%shared_ptr(sio::array_message)
%shared_ptr(sio::object_message)
%shared_ptr(sio::socket)

// template mapping
%template(MessageVector) std::vector<std::shared_ptr<sio::message>>;
%template(MessageMap) std::map<std::string, std::shared_ptr<sio::message>>;
%template(StringMap) std::map<std::string, std::string>;

// ignores
%ignore sio::message::list::list(string &&);
%ignore sio::message::list::list(nullptr_t);
%ignore sio::message::list::list(shared_ptr<const string> const&);
%ignore sio::string_message::create(string &&);
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

// map the delegates
%cs_callback(con_listener_delegate, con_listener);
%cs_callback(close_listener_delegate, close_listener);
%cs_callback(reconnect_listener_delegate, reconnect_listener);
%cs_callback(socket_listener_delegate, socket_listener);
%cs_callback(event_listener_delegate, event_listener);
%cs_callback(event_listener_aux_delegate, event_listener_aux);
%cs_callback(error_listener_delegate, error_listener);
%cs_callback(ack_delegate, ack_delegate);


// API definitions
// other types
typedef	long long int64_t;
typedef void (*con_listener_delegate)(void);
typedef void (*close_listener_delegate)(sio::client::close_reason const&);
typedef void (*reconnect_listener_delegate)(unsigned, unsigned);
typedef void (*socket_listener_delegate)(std::string const&);
typedef void (*event_listener_aux_delegate)(const std::string&, sio::message::ptr const&, bool, sio::message::ptr&);
typedef void (*event_listener_delegate)(sio::event&);
typedef void (*error_listener_delegate)(sio::message::ptr const&);
typedef void (*ack_delegate)(sio::message::list const&);
// include files
%include "../../out/include/sio_client.h"
%include "../../out/include/sio_message.h"
%include "../../out/include/sio_socket.h"


// extending types
namespace sio {
    %extend client {
        void set_open_listener(con_listener_delegate const& l) {
            $self->set_open_listener([l](void) {
                l();
            });
        }
        void set_fail_listener(con_listener_delegate const& l) {
            $self->set_fail_listener([l](void) {
                l();
            });
        }
        void set_reconnecting_listener(con_listener_delegate const& l) {
            $self->set_reconnecting_listener([l](void) {
                l();
            });
        }
        void set_reconnect_listener(reconnect_listener_delegate const& l) {
            $self->set_reconnect_listener([l](unsigned reconn_made, unsigned delay) {
                l(reconn_made, delay);
            });
        }
        void set_close_listener(close_listener_delegate const& l) {
            $self->set_close_listener([l](sio::client::close_reason const& reason) {
                l(reason);
            });
        }
        void set_socket_open_listener(socket_listener_delegate const& l) {
            $self->set_socket_open_listener([l](std::string const& nsp) {
                l(nsp);
            });
        }
        void set_socket_close_listener(socket_listener_delegate const& l) {
            $self->set_socket_close_listener([l](std::string const& nsp) {
                l(nsp);
            });
        }
    }
    %extend socket {
        void on(std::string const& event_name, event_listener const& func) {
            $self->on(event_name, [func](sio::event& event) {
                func(event);
            });
        }
        void on(std::string const& event_name, event_listener_aux_delegate const& l) {
            $self->on(event_name, [l](const std::string& name, sio::message::ptr const& message, bool need_ack, sio::message::ptr& ack_message) {
                l(name, message, need_ack, ack_message);
            });
        }
        void on_error(error_listener_delegate const& l) {
            $self->on_error([l](sio::message::ptr const& message) {
                l(message);
            });
        }
        void emit(std::string const& name, sio::message::list const& msglist, ack_delegate const& ack) {
            $self->emit(name, msglist, [ack](sio::message::list const& message_list) {
                ack(message_list);
            });
        }
    }
}

