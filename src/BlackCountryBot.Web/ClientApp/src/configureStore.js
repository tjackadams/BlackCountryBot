import { createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import { routerMiddleware } from "connected-react-router";
import { composeWithDevTools } from "redux-devtools-extension";
import {
  JsonHubProtocol,
  HttpTransportType,
  HubConnectionBuilder,
  LogLevel
} from "@aspnet/signalr";

import { createRootReducer } from "./store";

export function configureStore(history, initialState) {
  const composeEnhancers = composeWithDevTools({});

  const store = createStore(
    createRootReducer(history),
    initialState,
    composeEnhancers(
      applyMiddleware(thunk, routerMiddleware(history), signalRInvokeMiddleware)
    )
  );

  return store;
}
const connectionHub = "hub/phrases";
const protocol = new JsonHubProtocol();
const transport = HttpTransportType.WebSockets | HttpTransportType.LongPolling;

const options = {
  transport,
  logMessageContent: true,
  logger: LogLevel.Trace
};

const connection = new HubConnectionBuilder()
  .withUrl(connectionHub, options)
  .withHubProtocol(protocol)
  .build();

export function signalRInvokeMiddleware(store) {
  return next => async action => {
    switch (action.type) {
      case "SIGNALR_CREATE_PHRASE":
        connection.invoke("CreatePhrase", action.command);
        break;
      case "SIGNALR_DELETE_PHRASE":
        connection.invoke("DeletePhrase", action.command);
        break;
      case "SIGNALR_UPDATE_PHRASE":
        connection.invoke("UpdatePhrase", action.command);
        break;
      default:
        break;
    }
    return next(action);
  };
}

export function signalRRegisterCommands(store, callback) {
  connection.on("GetAllPhrases", data => {
    store.dispatch({ type: "GETALL_PHRASES", payload: data });
  });

  connection
    .start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err))
    .then(callback);
}
