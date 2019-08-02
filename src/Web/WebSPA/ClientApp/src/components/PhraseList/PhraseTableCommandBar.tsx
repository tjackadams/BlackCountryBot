import React from "react";
import { mergeStyleSets } from "@uifabric/styling";
import {
  CommandBarButton,
  IButtonProps,
  ICommandBarItemProps,
  CommandBar,
  getTheme
} from "office-ui-fabric-react";

interface IPhraseTableCommandBarClassNames {
  phraseListCommandBar: string;
  phraseListCommandBarButton: string;
}

const getClassNames = (): IPhraseTableCommandBarClassNames => {
  const theme = getTheme();
  return mergeStyleSets({
    phraseListCommandBar: {
      backgroundColor: theme.palette.neutralLight,
      marginBottom: 40
    },
    phraseListCommandBarButton: {
      backgroundColor: theme.palette.neutralLight,
      color: theme.palette.neutralDark
    }
  });
};

const customButton = React.memo((props: IButtonProps) => {
  const { phraseListCommandBarButton } = getClassNames();
  return <CommandBarButton {...props} className={phraseListCommandBarButton} />;
});

export class PhraseTableCommandBar extends React.Component<{
  items: ICommandBarItemProps[];
}> {
  public render(): JSX.Element {
    const theme = getTheme();
    const commandBarStyles = {
      root: {
        backgroundColor: theme.palette.neutralLight,
        color: theme.palette.neutralDark
      }
    };

    const { phraseListCommandBar } = getClassNames();
    return (
      <CommandBar
        className={phraseListCommandBar}
        buttonAs={customButton}
        items={this.props.items}
        styles={commandBarStyles}
      />
    );
  }
}
