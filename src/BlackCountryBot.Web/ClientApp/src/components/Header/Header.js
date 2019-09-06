import React, { useEffect, useState } from "react";
import axios from "axios";

import { DefaultPalette } from "office-ui-fabric-react/lib/Styling";
import { Image, ImageFit } from "office-ui-fabric-react/lib/Image";
import { Stack } from "office-ui-fabric-react/lib/Stack";
import { Text } from "office-ui-fabric-react/lib/Text";
import { Link } from "office-ui-fabric-react/lib/Link";
import { mergeStyles } from "@uifabric/merge-styles";

import flag from "./Black_Country_Flag.svg";

const stackStyles = {
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
    overflow: "visible"
  }
};
const stackTokens = {
  childrenGap: 0 + " " + 10,
  padding: "0px 10px 0px 10px"
};
const imageProps = {
  imageFit: ImageFit.contain
};

const linkStyles = mergeStyles({
  color: DefaultPalette.white,
  selectors: {
    ":hover": {
      color: DefaultPalette.themeDark,
      textDecoration: "none"
    },
    ":visited": {
      color: DefaultPalette.whiteTranslucent40
    }
  }
});

export const Header = props => {
  const [data, setData] = useState({ version: "1.0.0" });

  useEffect(() => {
    async function fetchData() {
      const result = await axios("/api/versions");

      setData({ version: result.data.value });
    }

    fetchData();
  }, []);

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
      <Stack.Item grow>
        <Text variant={"xxLarge"} block>
          {props.title}
        </Text>
      </Stack.Item>
      <Stack.Item
        styles={{
          root: {
            alignItems: "flex-end",
            display: "flex"
          }
        }}
      >
        <Text>
          Version:{" "}
          <Link
            href="https://github.com/tjackadams/BlackCountryBot/releases/latest"
            target="_blank"
            className={linkStyles}
          >
            {data.version}
          </Link>
        </Text>
      </Stack.Item>
    </Stack>
  );
};
