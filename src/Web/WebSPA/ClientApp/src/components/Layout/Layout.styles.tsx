import { mergeStyleSets, getTheme } from "@uifabric/styling";
import hexToRgba from "hex-to-rgba";

export interface ILayoutClassNames {
  contentItemContainer: string;
  contentContainer: string;
  main: string;
  headerContainer: string;
}

export const getClassNames = (): ILayoutClassNames => {
  const theme = getTheme();

  return mergeStyleSets({
    contentItemContainer: {
      backgroundColor: theme.palette.neutralLight
    },
    contentContainer: {
      marginTop: 40,
      paddingLeft: "16%",
      paddingRight: "16%",
      minHeight: "100vh"
    },
    main: {
      width: "calc(100vw - (100vw - 100%))"
    },
    headerContainer: {
      background: theme.palette.neutralLight,
      color: theme.palette.neutralDark,
      boxShadow: `0 1px 0 0 ${hexToRgba(theme.palette.neutralDark, 0.08)}`,
      zIndex: 10
    }
  });
};
