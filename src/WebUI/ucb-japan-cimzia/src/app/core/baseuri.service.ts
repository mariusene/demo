import { Injectable } from '@angular/core';

@Injectable()
export class BaseUriService {
    public getBaseUri():string{
        //TODO: read from configuration
        return `http://localhost:49487`;
    }
}