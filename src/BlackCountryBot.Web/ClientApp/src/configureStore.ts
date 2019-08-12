import {
  Store,
  createStore,
  applyMiddleware,
  Middleware,
  MiddlewareAPI,
  Dispatch
} from "redux";
import { routerMiddleware } from "connected-react-router";
import { composeWithDevTools } from "redux-devtools-extension";
import { History } from "history";
import {
  JsonHubProtocol,
  HttpTransportType,
  HubConnectionBuilder,
  LogLevel,
  HubConnection
} from "@aspnet/signalr";

import { ApplicationState, createRootReducer } from "./store";
import { PhrasesActionTypes } from "./store/phrases/types";

export default function configureStore(
  history: History,
  initialState: ApplicationState
): Store<ApplicationState> {
  const composeEnhancers = composeWithDevTools({});

  const store = createStore(
    createRootReducer(history),
    initialState,
    composeEnhancers(
      applyMiddleware(routerMiddleware(history), signlarMiddleware)
    )
  );

  return store;
}

type MyAction = { type: PhrasesActionTypes.GETALL };
type MyDispatch = Dispatch<MyAction>;

const startSignalRConnection = (connection: HubConnection) =>
  connection
    .start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err));

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

const signlarMiddleware: Middleware = (
  api: MiddlewareAPI<MyDispatch>
) => next => action => {
  connection.on("getAllPhrases", data => {
    api.dispatch({ type: PhrasesActionTypes.GETALL, payload: data });
  });

  return next(action);
};

startSignalRConnection(connection);
