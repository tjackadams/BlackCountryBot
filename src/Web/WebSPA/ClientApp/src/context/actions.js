import { types } from "./reducers";

export const useActions = (state, dispatch) => {
  function toggleDarkMode() {
    dispatch({ type: types.TOGGLE_DARK_MODE });
  }

  function setApplicationVersion(version) {
    dispatch({ type: types.SET_APPLICATION_VERSION, payload: version });
  }

  function getTranslations() {
    fetch(`${document.baseURI}api/configurations`)
      .then(res => res.json())
      .then(res => res.data)
      .then(res => {
        console.log("result: ", res);

        const botUrl = res.botUrl;

        return fetch(`${botUrl}/api/v1/translations`);
      })
      .then(res => res.json())
      .then(res =>
        dispatch({ type: types.SET_TRANSLATIONS, payload: res.data })
      );
  }

  return {
    toggleDarkMode,
    setApplicationVersion,
    getTranslations
  };
};
