import * as React from "react";
import { Provider } from "react-redux";
import { ConnectedRouter } from "connected-react-router";
import { Store } from "redux";
import { History } from "history";
import { Fabric } from "office-ui-fabric-react";

import Routes from "./routes";
import { ApplicationState } from "./store";
import { Layout } from "./components/Layout";

interface AppProps {
  store: Store<ApplicationState>;
  history: History;
}

const App: React.FC<AppProps> = ({ store, history }) => {
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
