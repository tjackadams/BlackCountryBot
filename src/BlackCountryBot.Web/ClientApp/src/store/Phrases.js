const getAllPhrasesType = "GETALL_PHRASES";
const createPhraseType = "CREATE_PHRASE";
const initialState = { phrases: [] };

export const actionCreators = {
  getAll: () => ({ type: getAllPhrasesType }),
  create: payload => ({
    type: "SIGNALR_CREATE_PHRASE",
    original: payload.original,
    translation: payload.translation
  }),
  delete: payload => ({
    type: "SIGNALR_DELETE_PHRASE",
    id: payload.id
  })
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === getAllPhrasesType) {
    return { ...state, phrases: action.payload };
  }

  return state;
};
