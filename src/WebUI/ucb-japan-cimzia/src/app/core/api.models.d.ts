
declare module api {
    interface CustomerMessage {
        firstName: string;
        lastName: string;
    }
    interface OrderMessage {
        customer: api.CustomerMessage;
        id: number;
        merchand: string;
    }
}
