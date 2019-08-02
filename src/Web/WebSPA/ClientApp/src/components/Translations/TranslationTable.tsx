import React, { useContext } from "react";
import { StoreContext } from "../../context/StoreContext";

export const TranslationTable: React.FC = () => {
  const { state } = useContext(StoreContext);

  return <div>table</div>;
};
