interface Event<TPayload> {
  type: string,
  payload: TPayload
}

interface EventCreator<TArgs extends any[], TPayload> {
  (...args: TArgs): Event<TPayload>
  type: string
}

export function createEvent<TArgs extends any[], TPayload>(
  type: string, createPayload: (...args: TArgs) => TPayload
) {
  const actionCreator = (...args: TArgs) => ({
    type, payload: createPayload(...args)
  });
  actionCreator.type = type;
  return actionCreator;
}