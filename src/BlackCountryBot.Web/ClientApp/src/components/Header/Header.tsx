import * as React from "react";

import {
  DefaultPalette,
  Image,
  IImageProps,
  ImageFit,
  Stack,
  IStackStyles,
  IStackTokens,
  Text
} from "office-ui-fabric-react";

import flag from "./Black_Country_Flag.svg";

export interface IHeaderProps {
  title: string;
}

export const Header: React.FC<IHeaderProps> = props => {
  const stackStyles: IStackStyles = {
    root: [
      {
        background: DefaultPalette.accent,
        color: DefaultPalette.white,
        marginLeft: 10,
        marginRight: 10,
        width: `calc(${100}% - 20px)`
      }
    ],
    inner: {
      overflow: "visible" as "hidden" | "visible"
    }
  };
  const stackTokens: IStackTokens = {
    childrenGap: 0 + " " + 10,
    padding: "0px 10px 0px 10px"
  };
  const imageProps: Partial<IImageProps> = {
    imageFit: ImageFit.contain
  };
  return (
    <Stack
      horizontal
      verticalAlign="center"
      styles={stackStyles}
      tokens={stackTokens}
    >
      <Stack.Item>
        <Image {...imageProps} src={flag} width={100} height={100} />
      </Stack.Item>
      <Stack.Item>
        <Text variant={"xxLarge"} block>
          {props.title}
        </Text>
      </Stack.Item>
    </Stack>
  );
};
