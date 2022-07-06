import React, { Component } from 'react';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { data: Object };
    }

    componentDidMount() {
        this.loadSession();
        this.updatePlayground();
    }

    async connect(sessionId) {
        const response = await fetch('connect?sessionId=' + sessionId, {
            method: "POST",
        });
        const data = await response.json();
        this.setState({ data: data });
    }

    async start() {
        const response = await fetch('start', {
            method: "POST",
        });
        const data = await response.json();
        this.setState({ data: data });
        console.log(this.state.data.Id + " " + this.state.data);
    }

    async setMark(x, y) {
        const response = await fetch('setMark?sessionId=' + this.state.data.Id + '&x=' + x + '&y=' + y, {
            method: "POST",
        });
        const data = await response.json();
        this.setState({ data: data });
    }

    renderPlayground(data) {
        let isNotCreated = this.state.data.Id == null;
        let content = isNotCreated ? (<div></div>) : (
            <table className='playground' aria-labelledby="tabelLabel">
                <tbody>
                    {data.Field.Cells.map((row) =>
                        <tr key={row[0].x}>{row.map((cell) =>
                            <td key={cell.x + "" + cell.y} onClick={() => this.setMark(cell.x, cell.y)}> {cell.Value}</td>)}
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
            //setState({ data: event.data });
            console.log('pss' + event.data);
            this.setState({ data: event.data });
        };
    }

    render() {
        let contents = 
            <div>
                <input type="number" id="sessionId"></input>
                <button id="connect" onClick={() => this.connect(document.getElementById("sessionId").value)}>Connect</button>
                <button id="start" onClick={() => this.start()}>Start</button>
                {console.log(this.state.data)}
                {this.state.data.Id == null ? (<div> </div>) : (this.renderPlayground(this.state.data))}
            </div>;

        return (contents);
    }

    async loadSession() {
        const response = await fetch('getSession?sessionId=' + this.state.data.Id);
        const data = await response.json();
        console.log(data);
        this.setState({ data: data });
    }
}
