import wrapPromise from "./wrapPromise";

function fetchTop5Tweets() {
  const promise = fetch(`${document.baseURI}api/configurations`)
    .then(res => res.json())
    .then(res => res.data)
    .then(res => {
      const botUrl = res.botUrl;

      return fetch(`${botUrl}/api/v1/translations?top=5`);
    })
    .then(res => res.json())
    .then(res => res.data);

  return wrapPromise(promise);
}

export { fetchTop5Tweets };
