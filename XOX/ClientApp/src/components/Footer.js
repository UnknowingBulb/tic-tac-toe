import React, { Component } from 'react';

export class Footer extends Component {

  render () {
    return (
        <footer>
            <div>Site use cookies. Leave if you don't agree with this.</div>
            {(window.location.href.includes('localhost')) === true ? <div>github repo <a href="https://github.com/UnknowingBulb/tic-tac-toe">tic-tac-toe</a> </div> : null }
        </footer>
    );
  }
}
