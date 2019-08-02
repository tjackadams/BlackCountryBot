import React, { useContext } from "react";

import { Toggle, Icon, TooltipHost } from "office-ui-fabric-react";
import { Image, ImageFit } from "office-ui-fabric-react/lib/Image";
import { Stack } from "office-ui-fabric-react/lib/Stack";
import { Text } from "office-ui-fabric-react/lib/Text";

import flag from "./Black_Country_Flag.svg";
import { getClassNames } from "./Header.styles";
import { StoreContext } from "../../context/StoreContext";

const imageProps = {
  imageFit: ImageFit.contain
};

export const Header: React.FC = () => {
  const { appName } = getClassNames();
  const { state, actions } = useContext(StoreContext);

  return (
    <Stack
      horizontal
      horizontalAlign="space-between"
      verticalAlign="center"
      tokens={{ childrenGap: 20 }}
      styles={{
        root: {
          paddingTop: 5,
          paddingBottom: 5,
          paddingRight: 20,
          paddingLeft: 20
        }
      }}
    >
      <Stack.Item>
        <Image {...imageProps} src={flag} width={100} height={100} />
      </Stack.Item>
      <Stack.Item grow>
        <Text variant="xxLarge" block className={appName}>
          `Black Country Twitter Bot v ${state.version}`
        </Text>
      </Stack.Item>
      <Stack.Item>
        <Toggle
          label={
            <div>
              Dark Mode{" "}
              <TooltipHost content="Dark Mode">
                <Icon iconName="Info" ariaLabel="Info tooltip" />
              </TooltipHost>
            </div>
          }
          inlineLabel
          onText="On"
          offText="Off"
          onChange={() => actions.toggleDarkMode()}
        />
      </Stack.Item>
    </Stack>
  );
};
