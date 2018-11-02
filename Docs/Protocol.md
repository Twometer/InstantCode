# InstantCode Protocol

## General
InstantCode uses a custom binary protocol, with is specs being as follows:

## Security
All network communication is AES encrypted with a pre-shared key.
This combines security and authorization into one, which makes sense
here because InstantCode is a self-hosting solution. The IV for the encryption
is randomized for each packet and prepended to the final packet.

## Packets

### General packet structure
```
| IV | Packet ID | Packet contents |
     | <----- AES ENCRYPTED -----> |
```
`IV` and `Packet contents` are byte arrays.

### Data types
- Boolean (one byte)
- Integer (four bytes)
- String (UTF-8 encoded and length-prefixed)
- Byte Array (always length-prefixed)

### Packets
#### 0x00 Login (C->S)
Used for identifying other users. An username can only be used once.
```
<String Username>
```

#### 0x01 State (S->C)
Used for state information, such as "Login OK" or "Username already taken". The payload is optional.
```
<Int ReasonCode><Int Payload>
```

##### Reason codes
- 0x00 Ok
- 0x01 UsernameTaken

#### 0x02 CreateSession (C->S)
Creates a coding session, responds with a `0x01 State` packet. If creation was successful,
a session id is returned in the State packet's payload, and the client starts transmitting
project data.
```
<String[] Participants>
```

#### 0x03 CloseSession (C->S)
Closes a coding session.
```
<Int SessionId>
```

#### 0x04 OpenStream (C<->S)
Opens a raw data stream. 
```
<Int DataLength>
```

#### 0x05 StreamData (C<->S)
Transmits raw data in a stream.
```
<Byte[] Data>
```

#### 0x06 CloseStream (C<->S)
Closes the currently open data stream.
```
No content
```

#### 0x07 CodeChange (C<->S)
Notifys other clients in a session about code changes.
```
<String Sender><String File><Int Index><String char>
```

#### 0x08 CursorPosition (C<->S)
Notifys other clients in a session about cursor position changes.
```
<String Sender><String File><Int NewIndex>
```

#### 0x09 Save (C<->S)
Forces everyone to save the project.
```
No content
```