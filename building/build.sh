#!/bin/bash

BTOUCH=/Developer/MonoTouch/usr/bin/btouch
SMCS=/Developer/MonoTouch/usr/bin/smcs
XBUILD=/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild
MONOXBUILD=/Library/Frameworks/Mono.framework/Commands/xbuild
CONFIGURATION=Debug


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
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphonesimulator -arch i386 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphonesimulator/libsioclient.a ios/libsioclient-i386.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphonesimulator -arch x86_64 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphonesimulator/libsioclient.a ios/libsioclient-x86_64.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphoneos -arch armv7 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphoneos/libsioclient.a ios/libsioclient-armv7.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphoneos -arch armv7s -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphoneos/libsioclient.a ios/libsioclient-armv7s.a &&
    $XBUILD -project sioclient.xcodeproj -target sioclient -sdk iphoneos -arch arm64 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphoneos/libsioclient.a ios/libsioclient-arm64.a)
# combine the arch
(cd external/ios &&
    lipo -create -output libsioclient.a libsioclient-i386.a libsioclient-x86_64.a libsioclient-armv7.a libsioclient-armv7s.a libsioclient-arm64.a)


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
    rm -rf src/* &&
    swig -csharp -c++ -dllimport "__Internal" -namespace "SocketIO" -outdir src -o src/sioclient_wrap.cxx sioclient.i &&
    sed -i.bak 's/public\ delegate/\[ObjCRuntime\.MonoNativeFunctionWrapper\]\ public\ delegate/g' src/sioclientPINVOKE.cs &&
    sed -i.bak 's/static\ void\ SetPendingArgument/static\ void\ TEMP_SetPendingArgument/g' src/sioclientPINVOKE.cs &&
    sed -i.bak 's/static\ void\ SetPending/\[ObjCRuntime\.MonoPInvokeCallback\(typeof\(ExceptionDelegate\)\)\]\ static\ void\ SetPending/g' src/sioclientPINVOKE.cs &&
    sed -i.bak 's/static\ void\ TEMP_SetPendingArgument/\[ObjCRuntime\.MonoPInvokeCallback\(typeof\(ExceptionArgumentDelegate\)\)\]\ static\ void\ SetPendingArgument/g' src/sioclientPINVOKE.cs &&
    sed -i.bak 's/static\ string\ CreateString/\[ObjCRuntime\.MonoPInvokeCallback\(typeof\(SWIGStringDelegate\)\)\]\ static\ string\ CreateString/g' src/sioclientPINVOKE.cs)

# build the C wrapper
mkdir wrapper/ios
(cd wrapper &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphonesimulator -arch i386 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphonesimulator/libwrapper.a ios/libwrapper-i386.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphonesimulator -arch x86_64 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphonesimulator/libwrapper.a ios/libwrapper-x86_64.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch armv7 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphoneos/libwrapper.a ios/libwrapper-armv7.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch armv7s -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphoneos/libwrapper.a ios/libwrapper-armv7s.a &&
    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch arm64 -configuration $CONFIGURATION clean build &&
    cp build/$CONFIGURATION-iphoneos/libwrapper.a ios/libwrapper-arm64.a)
# combine the arch
(cd wrapper/ios &&
    lipo -create -output libwrapper.a libwrapper-i386.a libwrapper-x86_64.a libwrapper-armv7.a libwrapper-armv7s.a libwrapper-arm64.a)
# copy all the libraries to the output folder
cp wrapper/ios/libwrapper.a out/lib/

# build the C# wrapper
(cd wrapper &&
    $MONOXBUILD /p:Configuration=$CONFIGURATION wrapper.sln)


## build the C wrapper
#mkdir wrapper-cppsharp/ios
#(cd wrapper-cppsharp &&
#    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphonesimulator -arch i386 -configuration $CONFIGURATION clean build &&
#    cp build/$CONFIGURATION-iphonesimulator/libwrapper.a ios/libwrapper-i386.a &&
#    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphonesimulator -arch x86_64 -configuration $CONFIGURATION clean build &&
#    cp build/$CONFIGURATION-iphonesimulator/libwrapper.a ios/libwrapper-x86_64.a &&
#    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch armv7 -configuration $CONFIGURATION clean build &&
#    cp build/$CONFIGURATION-iphoneos/libwrapper.a ios/libwrapper-armv7.a &&
#    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch armv7s -configuration $CONFIGURATION clean build &&
#    cp build/$CONFIGURATION-iphoneos/libwrapper.a ios/libwrapper-armv7s.a &&
#    $XBUILD -project wrapper.xcodeproj -target wrapper -sdk iphoneos -arch arm64 -configuration $CONFIGURATION clean build &&
#    cp build/$CONFIGURATION-iphoneos/libwrapper.a ios/libwrapper-arm64.a)
## combine the arch
#(cd wrapper-cppsharp/ios &&
#    lipo -create -output libwrapper.a libwrapper-i386.a libwrapper-x86_64.a libwrapper-armv7.a libwrapper-armv7s.a libwrapper-arm64.a)
## copy all the libraries to the output folder
#cp wrapper-cppsharp/ios/libwrapper.a out/lib/
## build the C# wrapper
#(cd wrapper-cppsharp &&
#    $MONOXBUILD /p:Configuration=$CONFIGURATION wrapper.sln)
#cp wrapper-cppsharp/wrapper/bin/$CONFIGURATION/wrapper.exe out/


# build the C# sample
(cd sample/SocketIOClientSample &&
    $MONOXBUILD /p:Configuration=$CONFIGURATION SocketIOClientSample.sln)
