/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.9.4.0 (NJsonSchema v10.3.1.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

import {GetManager} from "../OidcConfig";
export class AuthorizedApiBase {
    private readonly config: IConfig;

    protected constructor(config: IConfig) {
        this.config = config;
    }

    protected transformOptions = async (options: RequestInit): Promise<RequestInit> => {
        const user = await GetManager().getUser();
        if (user) {
            const token = `${user.token_type} ${user.access_token}`;
            options.headers = {
                ...options.headers,
                Authorization: token,
            };
        }
        return Promise.resolve(options);
    };
}

export class ApiClient extends AuthorizedApiBase {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(configuration: IConfig, baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super(configuration);
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

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
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

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
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
        } else if (status === 401) {
            return response.text().then((_responseText) => {
            return throwException("Unauthorized", status, _responseText, _headers);
            });
        } else if (status === 403) {
            return response.text().then((_responseText) => {
            return throwException("Forbidden", status, _responseText, _headers);
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
    processDailyData(signal?: AbortSignal | undefined): Promise<void> {
        let url_ = this.baseUrl + "/Api/ProcessDailyData";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
            }
        };

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
            return this.processProcessDailyData(_response);
        });
    }

    protected processProcessDailyData(response: Response): Promise<void> {
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
}

export class StockClient extends AuthorizedApiBase {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(configuration: IConfig, baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super(configuration);
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    /**
     * @return Success
     */
    list(signal?: AbortSignal | undefined): Promise<StockListItemDto[]> {
        let url_ = this.baseUrl + "/Stock/list";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
            return this.processList(_response);
        });
    }

    protected processList(response: Response): Promise<StockListItemDto[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <StockListItemDto[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<StockListItemDto[]>(<any>null);
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

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
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
     * @param type (optional) 
     * @param start (optional) 
     * @param end (optional) 
     * @return Success
     */
    history(symbol: string | null, type: HistoryType | undefined, start: Date | undefined, end: Date | undefined, signal?: AbortSignal | undefined): Promise<PriceHistory[]> {
        let url_ = this.baseUrl + "/Stock/{symbol}/history?";
        if (symbol === undefined || symbol === null)
            throw new Error("The parameter 'symbol' must be defined.");
        url_ = url_.replace("{symbol}", encodeURIComponent("" + symbol));
        if (type === null)
            throw new Error("The parameter 'type' cannot be null.");
        else if (type !== undefined)
            url_ += "type=" + encodeURIComponent("" + type) + "&";
        if (start === null)
            throw new Error("The parameter 'start' cannot be null.");
        else if (start !== undefined)
            url_ += "start=" + encodeURIComponent(start ? "" + start.toJSON() : "") + "&";
        if (end === null)
            throw new Error("The parameter 'end' cannot be null.");
        else if (end !== undefined)
            url_ += "end=" + encodeURIComponent(end ? "" + end.toJSON() : "") + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
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
     * @param date (optional) 
     * @return Success
     */
    daily(symbol: string | null, date: Date | null | undefined, signal?: AbortSignal | undefined): Promise<PriceDaily[]> {
        let url_ = this.baseUrl + "/Stock/{symbol}/daily?";
        if (symbol === undefined || symbol === null)
            throw new Error("The parameter 'symbol' must be defined.");
        url_ = url_.replace("{symbol}", encodeURIComponent("" + symbol));
        if (date !== undefined && date !== null)
            url_ += "date=" + encodeURIComponent(date ? "" + date.toJSON() : "") + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
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

export class UserPricingClient extends AuthorizedApiBase {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(configuration: IConfig, baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        super(configuration);
        this.http = http ? http : <any>window;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    /**
     * @param body (optional) 
     * @return Success
     */
    userPricing(body: AddUserShareCommand | undefined, signal?: AbortSignal | undefined): Promise<UserPrice> {
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

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
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
        } else if (status === 401) {
            return response.text().then((_responseText) => {
            return throwException("Unauthorized", status, _responseText, _headers);
            });
        } else if (status === 403) {
            return response.text().then((_responseText) => {
            return throwException("Forbidden", status, _responseText, _headers);
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
    list(signal?: AbortSignal | undefined): Promise<UserStock[]> {
        let url_ = this.baseUrl + "/UserPricing/list";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            signal,
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
            return this.processList(_response);
        });
    }

    protected processList(response: Response): Promise<UserStock[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <UserStock[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status === 401) {
            return response.text().then((_responseText) => {
            return throwException("Unauthorized", status, _responseText, _headers);
            });
        } else if (status === 403) {
            return response.text().then((_responseText) => {
            return throwException("Forbidden", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<UserStock[]>(<any>null);
    }

    /**
     * @return Success
     */
    shares(symbol: string | null, signal?: AbortSignal | undefined): Promise<UserPrice[]> {
        let url_ = this.baseUrl + "/UserPricing/{symbol}/shares";
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

        return this.transformOptions(options_).then(transformedOptions_ => {
            return this.http.fetch(url_, transformedOptions_);
        }).then((_response: Response) => {
            return this.processShares(_response);
        });
    }

    protected processShares(response: Response): Promise<UserPrice[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            result200 = _responseText === "" ? null : <UserPrice[]>JSON.parse(_responseText, this.jsonParseReviver);
            return result200;
            });
        } else if (status === 401) {
            return response.text().then((_responseText) => {
            return throwException("Unauthorized", status, _responseText, _headers);
            });
        } else if (status === 403) {
            return response.text().then((_responseText) => {
            return throwException("Forbidden", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<UserPrice[]>(<any>null);
    }
}

export interface StockListItemDto {
    symbol: string | null;
    longName: string | null;
    shortName: string | null;
    quoteType: string | null;
    currentPrice: number;
    currentVolume: number;
    currentTime: Date;
    status: string | null;
    currency: string | null;
    financialCurrency: string | null;
    market: string | null;
    timezone: string | null;
}

export interface Stock {
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

export enum HistoryType {
    Day = "Day",
    Month = "Month",
    Year = "Year",
}

export interface PriceHistory {
    stockSymbol: string | null;
    volume: number;
    opened: number;
    closed: number;
    high: number;
    low: number;
    dateTime: Date;
}

export interface PriceDaily {
    stockSymbol: string | null;
    ask: number | null;
    bid: number | null;
    askSize: number;
    bidSize: number;
    volume: number;
    price: number;
    time: Date;
}

export interface AddUserShareCommand {
    userId: string | null;
    symbol: string | null;
    price: number;
    count: number;
    dateTime: Date;
}

export interface UserPrice {
    userId: string | null;
    stockSymbol: string | null;
    price: number;
    count: number;
    dateTime: Date;
}

export interface UserStock {
    symbol: string | null;
    shares: number;
    total: number;
    totalProfit: number;
    shortName: string | null;
    longName: string | null;
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

/**
 * Configuration class needed in base class.
 * The config is provided to the API client at initialization time.
 * API clients inherit from #AuthorizedApiBase and provide the config.
 */
export class IConfig {
    /**
     * Returns a valid value for the Authorization header.
     * Used to dynamically inject the current auth header.
     */
    //getAuthorization!: () => 'the-authentication-token';
}