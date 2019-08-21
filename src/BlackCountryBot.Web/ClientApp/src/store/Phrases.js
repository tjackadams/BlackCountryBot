const getAllPhrasesType = "GETALL_PHRASES";
const initialState = { phrases: [] };

export const actionCreators = {
  getAll: () => ({ type: getAllPhrasesType }),
  create: payload => ({
    type: "SIGNALR_CREATE_PHRASE",
    command: payload
  }),
  delete: payload => ({
    type: "SIGNALR_DELETE_PHRASE",
    command: payload
  }),
  update: payload => ({
    type: "SIGNALR_UPDATE_PHRASE",
    command: payload
  }),
  tweet: payload => ({
    type: "SIGNALR_TWEET_PHRASE",
    command: payload
  })
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === getAllPhrasesType) {
    return { ...state, phrases: action.payload };
  }

  return state;
};
