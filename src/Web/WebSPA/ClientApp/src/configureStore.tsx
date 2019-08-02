import { createBrowserHistory } from "history";
import { createStore, applyMiddleware, Middleware } from "redux";
import { routerMiddleware } from "connected-react-router";
import { composeWithDevTools } from "redux-devtools-extension";
import {
  JsonHubProtocol,
  HttpTransportType,
  HubConnectionBuilder,
  LogLevel
} from "@aspnet/signalr";

import createRootReducer from "./store";

export const history = createBrowserHistory();

export function configureStore(preloadedState?: any) {
  const composeEnhancers = composeWithDevTools({});

  const store = createStore(
    createRootReducer(history),
    preloadedState,
    composeEnhancers(
      applyMiddleware(routerMiddleware(history), signalRInvokeMiddleware)
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

export default configureStore;

const connection = new HubConnectionBuilder()
  .withUrl(connectionHub, options)
  .withHubProtocol(protocol)
  .build();

export const signalRInvokeMiddleware: Middleware = api => next => async action => {
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
    case "SIGNALR_TWEET_PHRASE":
      connection.invoke("TweetPhrase", action.command);
      break;
    default:
      break;
  }
  return next(action);
};

export function signalRRegisterCommands(store: any, callback: any) {
  connection.on("GETALL_PHRASES", data => {
    store.dispatch({
      type: "@@phrase/PHRASES_LIST_UPDATED",
      payload: { phrases: data }
    });
  });

  connection.on("GET_TOP_TWEETS", data => {
    store.dispatch({
      type: "@@phrase/TOP_PHRASES_LIST_UPDATED",
      payload: {
        phrases: data
      }
    });
  });

  connection
    .start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err))
    .then(callback);
}
