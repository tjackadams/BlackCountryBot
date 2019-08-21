import React from "react";
import ReactDOM from "react-dom";
import { initializeIcons } from "office-ui-fabric-react/lib/Icons";
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

if (process.env.NODE_ENV !== "production") {
  console.log("configuring why-did-you-render");
  const whyDidYouRender = require("@welldone-software/why-did-you-render/dist/no-classes-transpile/umd/whyDidYouRender.min.js");
  whyDidYouRender(React, { include: [/^ConnectFunction$/, /Phrase/] });
}

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
