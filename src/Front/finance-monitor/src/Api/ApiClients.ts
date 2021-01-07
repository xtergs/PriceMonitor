/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.9.4.0 (NJsonSchema v10.3.1.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

export class ApiClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    /**
     * @param symbol (optional) 
     * @return Success
     */
    getStockDetails(symbol: string | null | undefined, signal?: AbortSignal | undefined): Promise<void> {
        let url_ = this.baseUrl + "/Api/GetStockDetails?";
        if (symbol !== undefined && symbol !== null)
            url_ += "symbol=" + encodeURIComponent("" + symbol) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetStockDetails(_response);
        });
    }

    protected processGetStockDetails(response: Response): Promise<void> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            return;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<void>(<any>null);
    }

    /**
     * @return Success
     */
    fillDb(signal?: AbortSignal | undefined): Promise<void> {
        let url_ = this.baseUrl + "/Api/FillDb";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processFillDb(_response);
        });
    }

    protected processFillDb(response: Response): Promise<void> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            return;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<void>(<any>null);
    }

    /**
     * @return Success
     */
    testAuth(signal?: AbortSignal | undefined): Promise<boolean> {
        let url_ = this.baseUrl + "/Api/TestAuth";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processTestAuth(_response);
        });
    }

    protected processTestAuth(response: Response): Promise<boolean> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <boolean>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<boolean>(<any>null);
    }
}

export class StockClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    /**
     * @return Success
     */
    list(signal?: AbortSignal | undefined): Promise<Stock[]> {
        let url_ = this.baseUrl + "/Stock/list";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processList(_response);
        });
    }

    protected processList(response: Response): Promise<Stock[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <Stock[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<Stock[]>(<any>null);
    }

    /**
     * @return Success
     */
    stock(symbol: string | null, signal?: AbortSignal | undefined): Promise<Stock> {
        let url_ = this.baseUrl + "/Stock/{symbol}";
        if (symbol === undefined || symbol === null)
            throw new Error("The parameter 'symbol' must be defined.");
        url_ = url_.replace("{symbol}", encodeURIComponent("" + symbol));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processStock(_response);
        });
    }

    protected processStock(response: Response): Promise<Stock> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <Stock>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<Stock>(<any>null);
    }

    /**
     * @return Success
     */
    history(symbol: string | null, signal?: AbortSignal | undefined): Promise<PriceHistory[]> {
        let url_ = this.baseUrl + "/Stock/{symbol}/history";
        if (symbol === undefined || symbol === null)
            throw new Error("The parameter 'symbol' must be defined.");
        url_ = url_.replace("{symbol}", encodeURIComponent("" + symbol));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processHistory(_response);
        });
    }

    protected processHistory(response: Response): Promise<PriceHistory[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <PriceHistory[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<PriceHistory[]>(<any>null);
    }

    /**
     * @return Success
     */
    daily(symbol: string | null, signal?: AbortSignal | undefined): Promise<PriceDaily[]> {
        let url_ = this.baseUrl + "/Stock/{symbol}/daily";
        if (symbol === undefined || symbol === null)
            throw new Error("The parameter 'symbol' must be defined.");
        url_ = url_.replace("{symbol}", encodeURIComponent("" + symbol));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processDaily(_response);
        });
    }

    protected processDaily(response: Response): Promise<PriceDaily[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <PriceDaily[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<PriceDaily[]>(<any>null);
    }
}

export class UserPricingClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    /**
     * @param body (optional) 
     * @return Success
     */
    userPricing(body: AddUserPriceDto | undefined, signal?: AbortSignal | undefined): Promise<UserPrice> {
        let url_ = this.baseUrl + "/UserPricing";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(body);

        let options_ = <RequestInit>{
            body: content_,
            method: "POST",
            signal,
            headers: {
                "Content-Type": "application/json",
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processUserPricing(_response);
        });
    }

    protected processUserPricing(response: Response): Promise<UserPrice> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <UserPrice>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<UserPrice>(<any>null);
    }

    /**
     * @return Success
     */
    list(stockId: string, signal?: AbortSignal | undefined): Promise<UserPrice[]> {
        let url_ = this.baseUrl + "/UserPricing/{stockId}/list";
        if (stockId === undefined || stockId === null)
            throw new Error("The parameter 'stockId' must be defined.");
        url_ = url_.replace("{stockId}", encodeURIComponent("" + stockId));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processList(_response);
        });
    }

    protected processList(response: Response): Promise<UserPrice[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <UserPrice[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<UserPrice[]>(<any>null);
    }
}

export interface Stock {
    id: string;
    symbol: string | null;
    market: string | null;
    timezone: string | null;
    time: Date;
    longName: string | null;
    shortName: string | null;
    language: string | null;
    currency: string | null;
    financialCurrency: string | null;
    quoteType: string | null;
}

export interface PriceHistory {
    id: string;
    stockId: string;
    volume: number;
    opened: number;
    closed: number;
    high: number;
    low: number;
    dateTime: Date;
}

export interface PriceDaily {
    id: string;
    stockId: string;
    ask: number | null;
    bid: number | null;
    askSize: number;
    bidSize: number;
    volume: number;
    price: number;
    time: Date;
}

export interface AddUserPriceDto {
    userId: string;
    symbol: string | null;
    price: number;
    count: number;
    dateTime: Date;
}

export interface UserPrice {
    id: string;
    userId: string;
    stockId: string;
    price: number;
    count: number;
    dateTime: Date;
}

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new ApiException(message, status, response, headers, null);
}