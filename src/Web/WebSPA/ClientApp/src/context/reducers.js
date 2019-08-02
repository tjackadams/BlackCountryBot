const initialState = {
  darkMode: false,
  version: "1.0.0",
  isTranslationsReady: false,
  translations: []
};

const types = {
  TOGGLE_DARK_MODE: "TOGGLE_DARK_MODE",
  SET_APPLICATION_VERSION: "SET_APPLICATION_VERSION",
  GET_TRANSLATIONS: "GET_TRANSLATIONS",
  SET_TRANSLATIONS: "SET_TRANSLATIONS"
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case types.TOGGLE_DARK_MODE:
      return {
        ...state,
        darkMode: !state.darkMode
      };
    case types.SET_APPLICATION_VERSION:
      return {
        ...state,
        version: action.payload
      };
    case types.SET_TRANSLATIONS:
      return {
        ...state,
        isTranslationsReady: true,
        translations: action.payload
      };
    default:
      throw new Error("Unexpected action");
  }
};

export { initialState, types, reducer };
