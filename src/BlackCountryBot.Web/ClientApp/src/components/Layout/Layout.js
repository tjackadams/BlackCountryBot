import React from "react";

import { DefaultPalette } from "office-ui-fabric-react/lib/Styling";
import { Stack } from "office-ui-fabric-react/lib/Stack";

import { Header } from "../Header";

export const Layout = props => {
  const stackStyles = {
    root: [
      {
        background: DefaultPalette.white,
        marginLeft: 10,
        marginRight: 10,
        minHeight: 100,
        width: "calc(100% - 20px)"
      }
    ],
    inner: {
      overflow: "visible"
    }
  };
  const stackTokens = {
    childrenGap: 0 + " " + 10,
    padding: "10px 10px 10px 10px"
  };
  return (
    <Stack>
      <Header title="Black Country Twitter Bot" />
      <Stack
        horizontal
        horizontalAlign="center"
        verticalAlign="start"
        styles={stackStyles}
        tokens={stackTokens}
      >
        {props.children}
      </Stack>
    </Stack>
  );
};
