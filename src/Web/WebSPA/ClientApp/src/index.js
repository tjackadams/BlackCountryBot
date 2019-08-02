import React from "react";
import ReactDOM from "react-dom";
import { initializeIcons } from "office-ui-fabric-react/lib/Icons";

import App from "./App";
import * as serviceWorker from "./serviceWorker";
// import {
//   configureStore,
//   history,
//   signalRRegisterCommands
// } from "./configureStore";

// const store = configureStore();

// initializeIcons();

// signalRRegisterCommands(store, () => {
//   ReactDOM.render(
//     <App store={store} history={history} />,
//     document.getElementById("root")
//   );
// });

const mountNode = document.querySelector("#root");

ReactDOM.createRoot(mountNode).render(<App />);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
