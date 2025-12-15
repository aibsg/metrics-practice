import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
    vus: 5,     
    iterations: 50,  
};

const BASE_URL = 'http://api:8080';

const endpoints = [
    '/fast',
    '/slow',
    '/random-delay',
    '/error',
];

export default function () {
    for (const path of endpoints) {
        http.get(`${BASE_URL}${path}`);
    }

    sleep(1);
}