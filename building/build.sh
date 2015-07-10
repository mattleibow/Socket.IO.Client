#!/bin/bash

BTOUCH=/Developer/MonoTouch/usr/bin/btouch
SMCS=/Developer/MonoTouch/usr/bin/smcs
XBUILD=/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild
MONOXBUILD=/Library/Frameworks/Mono.framework/Commands/xbuild


# download and build boost
chmod +x build-boost.sh
if [ ! -s external/boost ]; then
    mkdir external/boost
fi
(cd external/boost && ../../build-boost.sh)


# download and build socket.io
if [ -s external/socketio/src/.git ]; then
    (cd external/socketio/src && git reset --hard && git clean -xdf)
fi
if [ ! -s external/socketio/src/.git ]; then
    git clone --recurse-submodules https://github.com/socketio/socket.io-client-cpp.git external/socketio/src
fi
(cd external/socketio/src &&
    cmake -DBOOST_ROOT:STRING=../../boost/ios/prefix -DBOOST_VER:STRING=1.55.0 . \
    make install)


# build the iOS library
# build each arch
mkdir external/ios
(cd external &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphonesimulator -configuration Release clean build &&
    cp build/Release-iphonesimulator/libsioclient.a ios/libsioclient-i386.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphoneos -arch armv7 -configuration Release clean build &&
    cp build/Release-iphoneos/libsioclient.a ios/libsioclient-armv7.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphoneos -arch armv7s -configuration Release clean build &&
    cp build/Release-iphoneos/libsioclient.a ios/libsioclient-armv7s.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphoneos -arch arm64 -configuration Release clean build &&
    cp build/Release-iphoneos/libsioclient.a ios/libsioclient-arm64.a)
# combine the arch
(cd external/ios &&
    lipo -create -output libsioclient.a libsioclient-i386.a libsioclient-armv7.a libsioclient-armv7s.a libsioclient-arm64.a)


# copy all the libraries to the output folder
rm -rf out
mkdir out
mkdir out/lib
cp external/boost/ios/framework/boost.framework/boost out/lib/libboost.a
cp external/ios/libsioclient.a out/lib/
mkdir out/include
cp external/socketio/src/src/*.h out/include/


# create and build the C and C# wrapper
# create the wrappers
mkdir wrapper/src
(cd wrapper &&
    rm -rf src/*
    cp sioclient.i src/sioclient.i
    swig -csharp -c++ -dllimport "__Internal" -namespace "SocketIO.Internal" src/sioclient.i)
# build the C wrapper
mkdir wrapper/ios
(cd wrapper &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphonesimulator -configuration Release clean build &&
    cp build/Release-iphonesimulator/libwrapper.a ios/libwrapper-i386.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch armv7 -configuration Release clean build &&
    cp build/Release-iphoneos/libwrapper.a ios/libwrapper-armv7.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch armv7s -configuration Release clean build &&
    cp build/Release-iphoneos/libwrapper.a ios/libwrapper-armv7s.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch arm64 -configuration Release clean build &&
    cp build/Release-iphoneos/libwrapper.a ios/libwrapper-arm64.a)
# combine the arch
(cd wrapper/ios &&
    lipo -create -output libwrapper.a libwrapper-i386.a libwrapper-armv7.a libwrapper-armv7s.a libwrapper-arm64.a)
# build the C# wrapper


