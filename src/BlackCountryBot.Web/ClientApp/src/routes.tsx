import React, { Suspense } from "react";
import { Route, Switch } from "react-router-dom";

const IndexPage = React.lazy(() => import("./pages/index"));

const Routes: React.SFC = () => (
  <Switch>
    <Route exact path="/" component={WaitingComponent(IndexPage)} />
    <Route component={() => <div>Not Found</div>} />
  </Switch>
);

function WaitingComponent(Component) {
  return props => (
    <Suspense fallback={<div>Loading...</div>}>
      <Component {...props} />
    </Suspense>
  );
}

export default Routes;
