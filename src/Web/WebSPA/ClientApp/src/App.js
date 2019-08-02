import React from "react";
import { mergeStyles } from "@uifabric/styling";

import { Layout } from "./components/Layout";
import { Switch, Route } from "react-router";

import { IndexPage } from "./pages/index";
import { BrowserRouter } from "react-router-dom";
import { StoreProvider } from "./context/StoreContext";

mergeStyles({
  selectors: {
    ":global(body), :global(html)": {
      margin: 0,
      padding: 0,
      height: "100vh"
    }
  }
});

const App = () => {
  return (
    <StoreProvider>
      <BrowserRouter>
        <Switch>
          <Layout>
            <Route exact path="/" component={IndexPage} />
            <Route component={() => <div>Not Found</div>} />
          </Layout>
        </Switch>
      </BrowserRouter>
    </StoreProvider>
  );
};

export default App;
