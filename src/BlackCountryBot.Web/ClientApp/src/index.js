import React from "react";
import ReactDOM from "react-dom";
import { initializeIcons } from "office-ui-fabric-react";
import { createBrowserHistory } from "history";

import App from "./App";
import * as serviceWorker from "./serviceWorker";
import { configureStore, signalRRegisterCommands } from "./configureStore";

// Create browser history to use in the Redux store
//const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const baseUrl = (document.querySelector("base") || {}).href;
const history = createBrowserHistory({ basename: baseUrl });
const initialState = window.initialReduxState;
const store = configureStore(history, initialState);

initializeIcons();

signalRRegisterCommands(store, () => {
  ReactDOM.render(
    <App store={store} history={history} />,
    document.getElementById("root")
  );
});

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
