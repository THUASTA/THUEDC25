import asyncio
import websocket
import json
import time

async def send_websocket_message():
    # ws server uri
    uri = "ws://127.0.0.1:8080"

    # Message to send
    messages = {
        "start": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "START"
        },
        "end": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "END"
        },
        "reset": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "RESET"
        },
        "config": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "GET_HOST_CONFIGURATION"
        }
    }

    # Bad message to send
    bad_messages = {
        "empty": {

        },
        "key_missing": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string"
        },
        "wrong_key": {
            "asdgkladf": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "START"
        },
        "wrong_key_num": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "START",
            "sdfasdlf": 111
        },
        "wrong_value_type": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": 0
        },
        "wrong_command": {
            "messageType": "COMPETITION_CONTROL_COMMAND",
            "token": "string",
            "command": "sdklfadsnkfl"
        },
        "not_match": {
            "messageType": "HOST_CONFIGURATION_FROM_CLIENT",
            "token": "string",
            "command": "START"
        }
    }

    async def send(ws: websocket.WebSocket, message: str):
        # Send json
        async def sendtask():
            ws.send(json.dumps(message))
        await sendtask()

        # Wait for response
        response = ws.recv()

        print(f"Sent message: {json.dumps(message)}")
        print(f"Received response: {response}")

        time.sleep(3)
        return

    ws = websocket.WebSocket()
    ws.connect(uri)

    if ws.connected == True:
        # Send messages
        await send(ws, messages["start"])
        await send(ws, messages["end"])
        await send(ws, messages["reset"])
        await send(ws, messages["start"])
        await send(ws, messages["config"])

        # Send "START" command again
        await send(ws, messages["start"])

        #Send bad messages
        await send(ws, bad_messages["empty"])
        await send(ws, bad_messages["key_missing"])
        await send(ws, bad_messages["wrong_command"])
        await send(ws, bad_messages["wrong_key"])
        await send(ws, bad_messages["wrong_key_num"])
        await send(ws, bad_messages["wrong_value_type"])
        await send(ws, bad_messages["not_match"])

        time.sleep(2)

        # Close ws connection
        ws.close()

def main():
    asyncio.get_event_loop().run_until_complete(send_websocket_message())
    print("Done")

if __name__ == "__main__":
    main()
