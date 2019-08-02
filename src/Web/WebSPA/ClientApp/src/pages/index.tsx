import React, { useContext, useEffect } from "react";
import { Stack, Text, mergeStyleSets } from "office-ui-fabric-react";
import { TranslationTable } from "../components";
import { StoreContext } from "../context/StoreContext";

interface IIndexClassNames {
  chartTitleContainer: string;
}

const getClassNames = (): IIndexClassNames => {
  return mergeStyleSets({
    chartTitleContainer: {
      textAlign: "center"
    }
  });
};

export const IndexPage: React.FC = () => {
  const { actions } = useContext(StoreContext);
  const { chartTitleContainer } = getClassNames();

  useEffect(() => {
    actions.getTranslations();
  }, [actions]);

  return (
    <Stack tokens={{ childrenGap: 40 }}>
      <Stack.Item>
        <Stack tokens={{ childrenGap: 20 }}>
          <Stack.Item className={chartTitleContainer}>
            <Text variant="xxLarge" block>
              Latest Tweets
            </Text>
          </Stack.Item>
          <Stack.Item>volume chart</Stack.Item>
        </Stack>
      </Stack.Item>
      <Stack.Item>
        <TranslationTable />
      </Stack.Item>
    </Stack>
  );
};
