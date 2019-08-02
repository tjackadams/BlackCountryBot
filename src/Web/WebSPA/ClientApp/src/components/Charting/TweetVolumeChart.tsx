import React, { useEffect, useState } from "react";
import axios from "axios";
import {
  DonutChart,
  IDonutChartProps,
  IChartProps,
  IChartDataPoint
} from "@uifabric/charting";
import { DefaultPalette } from "office-ui-fabric-react";
import { useSelector } from "react-redux";
import { IPhrase } from "../../store/Phrases";
import { ApplicationState } from "../../store";

export const TweetVolumeChart: React.FC<IDonutChartProps> = props => {
  const [state, setState] = useState({ topTweets: [] });

  useEffect(() => {
    async function fetchData() {
      const baseUri = `${document.baseURI}api/configurations`;
      console.log(baseUri);

      const response = await axios(baseUri);

      console.log("response: ", response);

      const botApi = response.data["botUrl"];

      console.log("api url: ", botApi);

      const topResponse = await axios(`${botApi}/api/v1/translations?top=5`);
      const tweets = topResponse.data.map((tweet: IPhrase) => {
        return {
          legend: tweet.original,
          data: tweet.numberOfTweets,
          color: randomColor(randomIndex())
        };
      });

      setState({ topTweets: tweets });
    }

    fetchData();
  });

  // const [state, setState] = useState<{ topTweets: IChartDataPoint[] }>({
  //   topTweets: [
  //     {
  //       legend: "empty",
  //       data: 100,
  //       color: randomColor(randomIndex())
  //     }
  //   ]
  // });
  // const { top5: top5Tweets } = useSelector(
  //   (state: ApplicationState) => state.phrases
  // );
  // useEffect(() => {
  //   const tweets = top5Tweets.map((tweet: IPhrase) => {
  //     return {
  //       legend: tweet.original,
  //       data: tweet.numberOfTweets,
  //       color: randomColor(randomIndex())
  //     };
  //   });

  //   const equals =
  //     state.topTweets.length === tweets.length &&
  //     state.topTweets.every((e, i) => e.legend === tweets[i].legend);

  //   if (!equals) {
  //     setState({
  //       ...state,
  //       topTweets: tweets
  //     });
  //   }
  // }, [state, top5Tweets]);

  const chartTitle = "Top 5 Tweets";

  const data: IChartProps = {
    chartTitle: chartTitle,
    chartData: state.topTweets
  };

  if (state.topTweets) {
    return (
      <DonutChart
        data={data}
        innerRadius={30}
        href={"https://developer.microsoft.com/en-us/"}
      />
    );
  }

  return null;
};

const randomIndex = (): number => {
  return Math.floor(Math.random() * 4);
};

const randomColor = (index: number): string => {
  return colors[index][Math.floor(Math.random() * colors[index].length)];
};

const colors = [
  [
    DefaultPalette.blueLight,
    DefaultPalette.blue,
    DefaultPalette.tealLight,
    DefaultPalette.teal,
    DefaultPalette.greenLight
  ],
  [
    DefaultPalette.purpleLight,
    DefaultPalette.purple,
    DefaultPalette.magentaLight,
    DefaultPalette.magenta
  ],
  [
    DefaultPalette.yellowLight,
    DefaultPalette.yellow,
    DefaultPalette.orangeLighter,
    DefaultPalette.orangeLight
  ],
  [
    DefaultPalette.neutralTertiary,
    DefaultPalette.neutralSecondary,
    DefaultPalette.neutralPrimary
  ]
];
