import { mergeStyleSets, getTheme } from "@uifabric/styling";

export interface IHeaderClassNames {
  appName: string;
}

export const getClassNames = (): IHeaderClassNames => {
  const theme = getTheme();

  return mergeStyleSets({
    appName: {
      color: theme.palette.themePrimary,
    }
  });
};
