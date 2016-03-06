# Socket.IO Client

[![Build status](https://ci.appveyor.com/api/projects/status/qh8lw8q3btg0ia7g/branch/master?svg=true)](https://ci.appveyor.com/project/mattleibow/socket-io-client/branch/master)

Xamarin bindings for the Socket.IO Clients. Socket.IO for Xamarin

# Example Usage

## Import the Namespace
```
using SocketIO.Client;

SocketIO.Client.Socket socket;
```

In the ```OnCreate``` method or anyhwere else you would like, you can now make the connection
## Create the Socket
```
socket = IO.Socket ("http://chat.peruzal.co.za");
```

### Subscribe to Events
```
socket.On (Socket.EventConnect, (data) => {
    Log.Debug(TAG, "Connected");
    socket.Emit("add user", "Xamarin Android");
});
```

## Another Events
```
socket.On ("new message", (data) => {
    Log.Debug(TAG, data.ToString());
});
```

## Connect
```
socket.Connect ();
```
