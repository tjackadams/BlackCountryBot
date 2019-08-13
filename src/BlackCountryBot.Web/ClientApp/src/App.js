import * as React from "react";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { Fabric } from "office-ui-fabric-react";

import Routes from "./routes";
import { Layout } from "./components/Layout";

const App = ({ store, history }) => {
  return (
    <Fabric>
      <Provider store={store}>
        <ConnectedRouter history={history}>
          <Layout>
            <Routes />
          </Layout>
        </ConnectedRouter>
      </Provider>
    </Fabric>
  );
};

export default App;
