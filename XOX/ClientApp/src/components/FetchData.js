import React, { Component } from 'react';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { data: Object , error: ''};
    }

    componentDidMount() {
        this.updatePlayground();
    }

    async connect(sessionId) {
        const response = await fetch('connect?sessionId=' + sessionId, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async start() {
        const response = await fetch('start', {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async setMark(x, y) {
        const response = await fetch('setMark?sessionId=' + this.state.data.Id + '&x=' + x + '&y=' + y, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    renderPlayground(data) {
        let isNotCreated = false;
        let content = isNotCreated ? (<div></div>) : (
            <table className='playground' aria-labelledby='tabelLabel'>
                <tbody>
                    {data.Field.Cells.map((row) =>
                        <tr key={row[0].x}>{row.map((cell) =>
                            <td key={cell.x + '' + cell.y} onClick={() => this.setMark(cell.x, cell.y)}> {cell.Value}</td>)}
                        </tr>
                    )}
                </tbody>
            </table>
        );
        return content;
    }

    updatePlayground() {
        var source = new EventSource('/session');

        source.onmessage = function (event) {
            this.setState({ data: JSON.parse(event.data) });
        }.bind(this);
    }

    render() {
        let contents = 
            <div>
                <input type='number' id='sessionId'></input>
                <button id='connect' onClick={() => this.connect(document.getElementById('sessionId').value)}>Connect</button>
                <button id='start' onClick={() => this.start()}>Start</button>
                <div id='error' key='error' className='error'>{this.state.error}</div>
                {((this.state.data == null) || (this.state.data.Id == null)) ? (<div> </div>) : (this.renderPlayground(this.state.data))}
            </div>;

        return (contents);
    }

    async loadSession() {
        const response = await fetch('getSession?sessionId=' + this.state.data.Id);
        await this.proceedDataResponse(response);
    }

    async proceedDataResponse(response) {
        console.log(response);
        if (response.status >= 500) {
            this.setState({ data: this.state.data, error: "Internal server error: " + response.status });
        }
        else if (response.status >= 400) {
            this.setState({ data: this.state.data, error: await response.text() });
        }
        else {
            this.setState({ data: await response.json(), error: null });
        }
    }
}
