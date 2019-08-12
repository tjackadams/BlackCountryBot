import * as React from "react";
import * as ReactDOM from "react-dom";
import { initializeIcons } from "office-ui-fabric-react";
import { createBrowserHistory } from "history";

import App from "./App";
import * as serviceWorker from "./serviceWorker";
import configureStore from "./configureStore";

const history = createBrowserHistory();
const initialState = window.initialReduxState;
const store = configureStore(history, initialState);

initializeIcons();

ReactDOM.render(
  <App store={store} history={history} />,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
