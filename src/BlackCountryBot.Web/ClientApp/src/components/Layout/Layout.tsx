import * as React from "react";

import {
  DefaultPalette,
  Stack,
  IStackStyles,
  IStackTokens
} from "office-ui-fabric-react";

import { Header } from "../Header";

export const Layout: React.FC = props => {
  const stackStyles: IStackStyles = {
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
      overflow: "visible" as "hidden" | "visible"
    }
  };
  const stackTokens: IStackTokens = {
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
