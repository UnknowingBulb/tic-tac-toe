import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { data: Object, loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

    async connect(sessionId) {
        const response = await fetch('connect?sessionId=' + sessionId, {
            method: "POST",
        });
        const data = await response.json();
        this.setState({ data: data, loading: false });
    }

    async start() {
        const response = await fetch('start', {
            method: "POST",
        });
        const data = await response.json();
        this.setState({ data: data, loading: false });
    }

    async setMark(x, y) {
        const response = await fetch('setMark?sessionId=' + this.state.data.Id +'&x=' + x + '&y=' + y, {
            method: "POST",
        });
        const data = await response.json();
        this.setState({ data: data, loading: false });
    }

    renderForecastsTable(data) {
        console.log('pss'+ this.state.data.Id);
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

  render() {
    let isNotCreated = this.state.data.Id == null;
    let contents = this.state.loading
        ? 
      <div>
        <h1 id="tabelLabel">Wait pls</h1>
            <p><em>Loading...</em></p>
      </div>
        :
        <div>
            <input type="number" id="sessionId"></input>
            <button id="connect" onClick={() => this.connect(document.getElementById("sessionId").value)}>Connect</button>
            <button id="start" onClick={() => this.start()}>Start</button>
            {console.log(this.state.data)}
            {isNotCreated ? (<div></div>): (this.renderForecastsTable(this.state.data))}
        </div>;

    return (contents);
  }

  async populateWeatherData() {
      const response = await fetch('session?sessionId=' + this.state.data.Id);
      const data = await response.json();
      console.log(data);
    this.setState({ data: data, loading: false });
    }
}
