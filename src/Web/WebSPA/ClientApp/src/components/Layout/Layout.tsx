import React, { useContext } from "react";
import { Fabric, Stack } from "office-ui-fabric-react";

import { Header } from "../Header";
import { getClassNames } from "./Layout.styles";
import { loadTheme } from "@uifabric/styling";
import { StoreContext } from "../../context/StoreContext";

const lightTheme = {
  palette: {
    themePrimary: "#0078d4",
    themeLighterAlt: "#f3f9fd",
    themeLighter: "#d0e7f8",
    themeLight: "#a9d3f2",
    themeTertiary: "#5ca9e5",
    themeSecondary: "#1a86d9",
    themeDarkAlt: "#006cbe",
    themeDark: "#005ba1",
    themeDarker: "#004377",
    neutralLighterAlt: "#f8f8f8",
    neutralLighter: "#f4f4f4",
    neutralLight: "#eaeaea",
    neutralQuaternaryAlt: "#dadada",
    neutralQuaternary: "#d0d0d0",
    neutralTertiaryAlt: "#c8c8c8",
    neutralTertiary: "#bab8b7",
    neutralSecondary: "#a3a2a0",
    neutralPrimaryAlt: "#8d8b8a",
    neutralPrimary: "#323130",
    neutralDark: "#605e5d",
    black: "#494847",
    white: "#ffffff"
  }
};

const darkTheme = {
  palette: {
    themePrimary: "#98c6ff",
    themeLighterAlt: "#06080a",
    themeLighter: "#182029",
    themeLight: "#2e3c4d",
    themeTertiary: "#5c7799",
    themeSecondary: "#87afe0",
    themeDarkAlt: "#a3cdff",
    themeDark: "#b1d4ff",
    themeDarker: "#c6e0ff",
    neutralLighterAlt: "#201f1e",
    neutralLighter: "#201f1e",
    neutralLight: "#1e1e1d",
    neutralQuaternaryAlt: "#1c1b1b",
    neutralQuaternary: "#1b1a19",
    neutralTertiaryAlt: "#1a1918",
    neutralTertiary: "#c8c8c8",
    neutralSecondary: "#d0d0d0",
    neutralPrimaryAlt: "#dadada",
    neutralPrimary: "#ffffff",
    neutralDark: "#f4f4f4",
    black: "#f8f8f8",
    white: "#201f1e"
  }
};

export const Layout: React.FC = props => {
  const { state } = useContext(StoreContext);
  let theme = state.darkMode ? darkTheme : lightTheme;

  loadTheme(theme);

  const {
    contentItemContainer,
    contentContainer,
    main,
    headerContainer
  } = getClassNames();

  return (
    <Fabric>
      <Stack className={main}>
        <Stack.Item className={headerContainer}>
          <Header />
        </Stack.Item>
        <Stack.Item className={contentItemContainer}>
          <Stack
            horizontal
            horizontalAlign="center"
            verticalAlign="start"
            className={contentContainer}
          >
            {props.children}
          </Stack>
        </Stack.Item>
      </Stack>
    </Fabric>
  );
};
