export interface Event<TPayload = {}> {
  type: string,
  time: Date
  payload: TPayload
}

export interface EventCreator<TArgs extends any[] = any[], TPayload = {}> {
  (...args: TArgs): Event<TPayload>
  type: string
}

export function createEvent<TArgs extends any[], TPayload>(
  type: string, createPayload: (...args: TArgs) => TPayload
) {
  const actionCreator = (...args: TArgs) => ({
    type, time: new Date(), payload: createPayload(...args)
  });
  actionCreator.type = type;
  return actionCreator;
}

export function isOfType<TArgs extends any[] = any[], TPayload = {}>(e: Event<TPayload>, ec: EventCreator<TArgs, TPayload>){
  return e.type === ec.type;
}